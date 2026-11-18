using backend.Controllers;
using backend.DataStructures;
using backend.Enums;
using backend.Repositories;
using backend.Services;
using ChatMessage = backend.Models.Chat;
using ChatMessageDto = backend.Models.ChatMessageDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace nivra.api.Tests.Chat;

public class ChatEndpointTests
{
    private static (ChatController Controller, AppDbContext Context, BackgroundTaskQueue Queue, ChatService Service)
        CreateController()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        context.Users.Add(new backend.Models.User { Id = 1, Username = "tester", PasswordHash = "x" });
        context.SaveChanges();

        var queue = new BackgroundTaskQueue();
        var repository = new ChatRepo(context);
        var service = new ChatService(repository, CreateSkinService(context));

        return (new ChatController(queue, service), context, queue, service);
    }

    private static SkinService CreateSkinService(AppDbContext context)
    {
        var skinRepository = new ShopRepository<backend.Models.Skin>(context);
        skinRepository.Add(new backend.Models.Skin { Id = 3, Name = "Explorer", Price = 0 });
        return new SkinService(skinRepository, new ShopRepository<backend.Models.Order>(context));
    }

    private static ChatMessage ValidMessage(long userId = 1)
    {
        return new ChatMessage
        {
            Message = "Hello my health-nuts",
            UserId = userId,
            DateT = default
        };
    }

    [Fact]
    public void SendMessage_ReturnsAccepted_WhenMessageIsValid()
    {
        var (controller, _, _, _) = CreateController();

        var result = controller.SendUserMessage(ValidMessage());

        var accepted = Assert.IsType<AcceptedResult>(result);
        Assert.Equal(202, accepted.StatusCode);
    }

    [Fact]
    public void SendMessage_EnqueuesTheMessageForBackgroundProcessing()
    {
        var (controller, _, queue, _) = CreateController();
        var message = ValidMessage(userId: 42);

        controller.SendUserMessage(message);

        Assert.True(queue.TryDequeue(out var queued));
        Assert.Same(message, queued);
        Assert.Equal("Hello my health-nuts", queued.Message);
        Assert.Equal(42, queued.UserId);
    }

    [Fact]
    public void SendMessage_DoesNotPersistTheMessageSynchronously()
    {
        var (controller, context, _, _) = CreateController();

        controller.SendUserMessage(ValidMessage());

        Assert.Empty(context.Chat);
    }

    [Fact]
    public void SendMessage_DefersValidationToTheBackgroundWorker()
    {
        var (controller, _, _, _) = CreateController();
        var message = ValidMessage();
        message.Message = "";

        // Validation now happens in the background worker, so the endpoint accepts it.
        var result = controller.SendUserMessage(message);

        Assert.IsType<AcceptedResult>(result);
    }

    [Fact]
    public async Task QueuedMessage_IsSavedWithServerTime()
    {
        var (controller, context, queue, service) = CreateController();
        var message = ValidMessage();
        message.DateT = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        controller.SendUserMessage(message);
        Assert.True(queue.TryDequeue(out var queued));

        var status = await service.SendMessageToChat(queued, CancellationToken.None);

        Assert.Equal(SendStatus.Success, status);
        var savedChat = Assert.Single(context.Chat);
        Assert.True(
            Math.Abs((DateTime.UtcNow - savedChat.DateT).TotalSeconds) < 5,
            $"Expected server time, got {savedChat.DateT:O}");
    }

    [Fact]
    public async Task QueuedEmptyMessage_IsRejectedByTheBackgroundService()
    {
        var (controller, context, queue, service) = CreateController();
        var message = ValidMessage();
        message.Message = "";

        controller.SendUserMessage(message);
        Assert.True(queue.TryDequeue(out var queued));

        var status = await service.SendMessageToChat(queued, CancellationToken.None);

        Assert.Equal(SendStatus.MessageIsEmpty, status);
        Assert.Empty(context.Chat);
    }

    private static void SeedMessages(AppDbContext context, int count)
    {
        var start = new DateTime(2026, 10, 1, 9, 0, 0);
        for (var i = 1; i <= count; i++)
        {
            context.Chat.Add(new ChatMessage
            {
                DateT = start.AddMinutes(i),
                Message = $"message {i}",
                UserId = 1
            });
        }
        context.SaveChanges();
    }

    [Fact]
    public async Task GetMessages_ReturnsOkWithMessagesOldestFirst()
    {
        var (controller, context, _, _) = CreateController();
        SeedMessages(context, 3);

        var result = await controller.GetMessages(null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var messages = Assert.IsAssignableFrom<List<ChatMessageDto>>(ok.Value);
        Assert.Equal(
            new[] { "message 1", "message 2", "message 3" },
            messages.Select(message => message.Message));
    }

    [Fact]
    public async Task GetMessages_UsesDefaultWindowWhenCountIsOmitted()
    {
        var (controller, context, _, _) = CreateController();
        SeedMessages(context, 60);

        var result = await controller.GetMessages(null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var messages = Assert.IsAssignableFrom<List<ChatMessageDto>>(ok.Value);
        Assert.Equal(50, messages.Count);
    }

    [Fact]
    public async Task GetMessages_ReturnsRequestedNumberOfMessages()
    {
        var (controller, context, _, _) = CreateController();
        SeedMessages(context, 5);

        var result = await controller.GetMessages(2);

        var ok = Assert.IsType<OkObjectResult>(result);
        var messages = Assert.IsAssignableFrom<List<ChatMessageDto>>(ok.Value);
        Assert.Equal(
            new[] { "message 4", "message 5" },
            messages.Select(message => message.Message));
    }

    [Fact]
    public async Task GetMessages_ReturnsEmptyListWhenThereIsNoHistory()
    {
        var (controller, _, _, _) = CreateController();

        var result = await controller.GetMessages(null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var messages = Assert.IsAssignableFrom<List<ChatMessageDto>>(ok.Value);
        Assert.Empty(messages);
    }
}
