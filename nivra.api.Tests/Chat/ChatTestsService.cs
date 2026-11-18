using backend.Enums;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace nivra.api.Tests.Chat;

public class ChatTestsService
{
    
    [Fact]
    public async Task ServiceSavesNewMessage()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new ChatRepo(context);
        var service = new ChatService(repository);
        
        var chatMessage = new backend.Models.Chat
        {
            Id = 4,
            DateT = default,
            Message = "Hello my health-nuts",
            UserId = 0
        };

        var value = await service.SendMessageToChat(chatMessage, CancellationToken.None);
        Assert.Equal(SendStatus.Success, value);

    }

    private static (ChatService Service, ChatRepo Repo) CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        context.Users.Add(new backend.Models.User { Id = 1, Username = "tester", PasswordHash = "x" });
        context.SaveChanges();

        var repository = new ChatRepo(context);
        return (new ChatService(repository), repository);
    }

    private static async Task SeedMessages(ChatRepo repository, int messageCount)
    {
        var start = new DateTime(2026, 10, 1, 9, 0, 0);
        for (var i = 1; i <= messageCount; i++)
        {
            await repository.Add(new backend.Models.Chat
            {
                DateT = start.AddMinutes(i),
                Message = $"message {i}",
                UserId = 1
            }, CancellationToken.None);
        }
    }

    [Fact]
    public async Task ServiceReturnsMessagesOldestFirst()
    {
        var (service, repository) = CreateService();
        await repository.Add(new backend.Models.Chat
        {
            DateT = new DateTime(2026, 10, 2, 9, 0, 0),
            Message = "middle",
            UserId = 1
        }, CancellationToken.None);
        await repository.Add(new backend.Models.Chat
        {
            DateT = new DateTime(2026, 10, 3, 9, 0, 0),
            Message = "newest",
            UserId = 1
        }, CancellationToken.None);
        await repository.Add(new backend.Models.Chat
        {
            DateT = new DateTime(2026, 10, 1, 9, 0, 0),
            Message = "oldest",
            UserId = 1
        }, CancellationToken.None);

        var messages = await service.GetMessagesAsync();

        Assert.Equal(
            new[] { "oldest", "middle", "newest" },
            messages.Select(message => message.Message));
    }

    [Fact]
    public async Task ServiceReturnsEmptyListWhenThereIsNoHistory()
    {
        var (service, _) = CreateService();

        var messages = await service.GetMessagesAsync();

        Assert.Empty(messages);
    }

    [Fact]
    public async Task ServiceReturnsNewestMessagesWhenGivenAnExplicitCount()
    {
        var (service, repository) = CreateService();
        await SeedMessages(repository, 5);

        var messages = await service.GetMessagesAsync(2);

        Assert.Equal(
            new[] { "message 4", "message 5" },
            messages.Select(message => message.Message));
    }

    [Fact]
    public async Task ServiceUsesFiftyMessagesWhenNoCountIsGiven()
    {
        var (service, repository) = CreateService();
        await SeedMessages(repository, 60);

        var messages = await service.GetMessagesAsync();

        Assert.Equal(50, messages.Count);
        Assert.Equal("message 11", messages.First().Message);
        Assert.Equal("message 60", messages.Last().Message);
    }

    [Fact]
    public async Task ServiceCapsRequestsAboveOneHundredMessages()
    {
        var (service, repository) = CreateService();
        await SeedMessages(repository, 101);

        var messages = await service.GetMessagesAsync(1000);

        Assert.Equal(100, messages.Count);
        Assert.Equal("message 2", messages.First().Message);
        Assert.Equal("message 101", messages.Last().Message);
    }
}
