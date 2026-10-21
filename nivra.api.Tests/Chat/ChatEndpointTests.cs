using System.Reflection;
using backend.Controllers;
using ChatMessage = backend.Models.Chat;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace nivra.api.Tests.Chat;

public class ChatEndpointTests
{
    private static (ChatController Controller, AppDbContext Context) CreateController()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new ChatRepo(context);
        var service = new ChatService(repository);

        return (new ChatController(service), context);
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
    public void SendMessage_ReturnsOk_WhenMessageIsValid()
    {
        var (controller, _) = CreateController();

        var result = controller.SendUserMessage(ValidMessage());

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public void SendMessage_SavesMessageForTheSendingUser()
    {
        var (controller, context) = CreateController();

        controller.SendUserMessage(ValidMessage(userId: 42));

        var savedChat = Assert.Single(context.Chat);
        Assert.Equal("Hello my health-nuts", savedChat.Message);
        Assert.Equal(42, savedChat.UserId);
    }

    [Fact]
    public void SendMessage_UsesServerTimeInsteadOfClientTime()
    {
        var (controller, context) = CreateController();

        controller.SendUserMessage(ValidMessage());

        var savedChat = Assert.Single(context.Chat);
        Assert.True(
            Math.Abs((DateTime.UtcNow - savedChat.DateT).TotalSeconds) < 5,
            $"Expected server time, got {savedChat.DateT:O}");
    }

    [Fact]
    public void SendMessage_ReturnsBadRequest_WhenMessageIsEmpty()
    {
        var (controller, _) = CreateController();
        var message = ValidMessage();
        message.Message = "";

        var result = controller.SendUserMessage(message);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public void SendMessage_ReturnsBadRequest_WhenMessageIsLongerThan200Characters()
    {
        var (controller, _) = CreateController();
        var message = ValidMessage();
        message.Message = new string('F', 201);

        var result = controller.SendUserMessage(message);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public void SendMessage_ReturnsBadRequest_WhenMessageIsNull()
    {
        var (controller, context) = CreateController();
        var message = ValidMessage();
        message.Message = null;

        var result = controller.SendUserMessage(message);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
        Assert.Empty(context.Chat);
    }

    [Fact]
    public void SendMessage_ReturnsBadRequest_WhenMessageIsOnlyWhitespace()
    {
        var (controller, context) = CreateController();
        var message = ValidMessage();
        message.Message = "     ";

        var result = controller.SendUserMessage(message);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
        Assert.Empty(context.Chat);
    }

    [Fact]
    public void SendMessage_StillReturnsOkAtExactly200Characters()
    {
        var (controller, context) = CreateController();
        var message = ValidMessage();
        message.Message = new string('F', 200);

        var result = controller.SendUserMessage(message);

        Assert.IsType<OkObjectResult>(result);
        Assert.Single(context.Chat);
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
        var (controller, context) = CreateController();
        SeedMessages(context, 3);

        var result = await controller.GetMessages(null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var messages = Assert.IsAssignableFrom<List<ChatMessage>>(ok.Value);
        Assert.Equal(
            new[] { "message 1", "message 2", "message 3" },
            messages.Select(message => message.Message));
    }

    [Fact]
    public async Task GetMessages_UsesDefaultWindowWhenCountIsOmitted()
    {
        var (controller, context) = CreateController();
        SeedMessages(context, 60);

        var result = await controller.GetMessages(null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var messages = Assert.IsAssignableFrom<List<ChatMessage>>(ok.Value);
        Assert.Equal(50, messages.Count);
    }

    [Fact]
    public async Task GetMessages_ReturnsRequestedNumberOfMessages()
    {
        var (controller, context) = CreateController();
        SeedMessages(context, 5);

        var result = await controller.GetMessages(2);

        var ok = Assert.IsType<OkObjectResult>(result);
        var messages = Assert.IsAssignableFrom<List<ChatMessage>>(ok.Value);
        Assert.Equal(
            new[] { "message 4", "message 5" },
            messages.Select(message => message.Message));
    }

    [Fact]
    public async Task GetMessages_ReturnsEmptyListWhenThereIsNoHistory()
    {
        var (controller, _) = CreateController();

        var result = await controller.GetMessages(null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var messages = Assert.IsAssignableFrom<List<ChatMessage>>(ok.Value);
        Assert.Empty(messages);
    }

}
