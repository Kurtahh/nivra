using Microsoft.EntityFrameworkCore;

namespace nivra.api.Tests.Chat;
using backend.Enums;
using backend.Models;
using backend.Repositories;

public class ChatTestsRepo
{
    
    [Fact]
    public async Task ChatMessageSent()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new ChatRepo(context);
        
        var chatMessage = new Chat
        {
            Id = 5,
            DateT = default,
            Message = "Hello my health-nuts",
            UserId = 0
        };
        var value = repository.Add(chatMessage);
        
        Assert.Equal(SendStatus.Success, value);
        
    }
    
    
    [Fact]
    public async Task AddSavesNewMessage()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new ChatRepo(context);
        
        var chatMessage = new Chat
        {
            Id = 4,
            DateT = default,
            Message = "Hello my health-nuts",
            UserId = 0
        };
        
        var value = repository.Add(chatMessage);
        Assert.Equal(SendStatus.Success, value);

        var savedChat = await context.Chat.FindAsync(4);
        Assert.NotNull(savedChat);
        Assert.Equal("Hello my health-nuts", savedChat.Message);
    }
    
    [Fact]
    public async Task AddFailsWhenMessageEmpty()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new ChatRepo(context);
        
        var chatMessage = new Chat
        {
            Id = 4,
            DateT = default,
            Message = "",
            UserId = 0
        };
        
        var value = repository.Add(chatMessage);

        Assert.Equal(SendStatus.MessageIsEmpty, value);
    }
    
    [Fact]
    public async Task AddFailsWhenMessageIsNull()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new ChatRepo(context);

        var chatMessage = new Chat
        {
            Id = 4,
            DateT = default,
            Message = null,
            UserId = 0
        };

        var value = repository.Add(chatMessage);

        Assert.Equal(SendStatus.MessageIsEmpty, value);
        Assert.Equal(0, context.Chat.Count());
    }

    [Fact]
    public async Task AddFailsWhenMessageIsOnlyWhitespace()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new ChatRepo(context);

        var chatMessage = new Chat
        {
            Id = 4,
            DateT = default,
            Message = "     ",
            UserId = 0
        };

        var value = repository.Add(chatMessage);

        Assert.Equal(SendStatus.MessageIsEmpty, value);
        Assert.Equal(0, context.Chat.Count());
    }

    [Fact]
    public async Task AddFailsWhenMessageIsLongerThan200Symbols()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new ChatRepo(context);
        var chatMessage = new Chat
        {
            Id = 4,
            DateT = default,
            Message = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" +
                      "FFFFFFFFFF" +
                      "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" +
                      "FFFFFFFFFFFFFFFFFFFFFF" +
                      "FFFFFFFF",
            UserId = 0
        };
        
        var value = repository.Add(chatMessage);

        Assert.Equal(SendStatus.MessageAbove200, value);
    }
    
    [Fact]
    public async Task AddFailsWhenMessageAlreadyExists()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new ChatRepo(context);
        var chatMessage = new Chat
        {
            Id = 4,
            DateT = default,
            Message = "abc",
            UserId = 0
        };
        var chatMessage2= new Chat
        {
            Id = 4,
            DateT = default,
            Message = "abcdef",
            UserId = 0
        };
        
        var value = repository.Add(chatMessage);
        Assert.Equal(SendStatus.Success, value);
        value = repository.Add(chatMessage2);
        Assert.Equal(SendStatus.MessageAlreadyExists, value);
        
    }

    private static (ChatRepo Repo, AppDbContext Context) CreateRepo()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        return (new ChatRepo(context), context);
    }

    private static backend.Models.Chat NewMessage(string text, DateTime dateT)
    {
        return new backend.Models.Chat
        {
            DateT = dateT,
            Message = text,
            UserId = 1
        };
    }

    [Fact]
    public async Task GetMessagesReturnsOldestFirst()
    {
        var (repository, _) = CreateRepo();
        var newest = NewMessage("newest", new DateTime(2026, 10, 3, 9, 0, 0));
        var oldest = NewMessage("oldest", new DateTime(2026, 10, 1, 9, 0, 0));
        var middle = NewMessage("middle", new DateTime(2026, 10, 2, 9, 0, 0));

        repository.Add(newest);
        repository.Add(oldest);
        repository.Add(middle);

        var messages = await repository.GetMessagesAsync(10);

        Assert.Equal(
            new[] { "oldest", "middle", "newest" },
            messages.Select(message => message.Message));
    }

    [Fact]
    public async Task GetMessagesReturnsEmptyListWhenThereAreNoMessages()
    {
        var (repository, _) = CreateRepo();

        var messages = await repository.GetMessagesAsync(10);

        Assert.Empty(messages);
    }

    [Fact]
    public async Task GetMessagesReturnsOnlyTheNewestMessagesUpToTheLimit()
    {
        var (repository, _) = CreateRepo();
        for (var day = 1; day <= 5; day++)
        {
            repository.Add(NewMessage($"day {day}", new DateTime(2026, 10, day, 9, 0, 0)));
        }

        var messages = await repository.GetMessagesAsync(2);

        Assert.Equal(
            new[] { "day 4", "day 5" },
            messages.Select(message => message.Message));
    }

    [Fact]
    public async Task GetMessagesUsesIdAsTiebreakerWhenTimestampsAreEqual()
    {
        var (repository, _) = CreateRepo();
        var sameTime = new DateTime(2026, 10, 1, 9, 0, 0);
        var laterInserted = NewMessage("second by id", sameTime);
        laterInserted.Id = 12;
        var earlierInserted = NewMessage("first by id", sameTime);
        earlierInserted.Id = 11;

        repository.Add(laterInserted);
        repository.Add(earlierInserted);

        var messages = await repository.GetMessagesAsync(10);

        Assert.Equal(
            new[] { "first by id", "second by id" },
            messages.Select(message => message.Message));
    }

    [Fact]
    public async Task GetMessagesKeepsTheNewestMessagesWhenTheLimitCutsThroughATie()
    {
        var (repository, _) = CreateRepo();
        var sameTime = new DateTime(2026, 10, 1, 9, 0, 0);
        foreach (var id in new[] { 21, 22, 23 })
        {
            var message = NewMessage($"id {id}", sameTime);
            message.Id = id;
            repository.Add(message);
        }

        var messages = await repository.GetMessagesAsync(2);

        Assert.Equal(
            new[] { "id 22", "id 23" },
            messages.Select(message => message.Message));
    }

    [Fact]
    public async Task GetMessagesDoesNotChangeTheStoredMessages()
    {
        var (repository, context) = CreateRepo();
        repository.Add(NewMessage("only", new DateTime(2026, 10, 1, 9, 0, 0)));

        await repository.GetMessagesAsync(10);

        Assert.Equal(1, context.Chat.Count());
    }
    
    [Fact]
    public async Task GetMessagesReturnsEmptyListIfCount0OrLess()
    {
        var (repository, context) = CreateRepo();
        repository.Add(NewMessage("only", new DateTime(2026, 10, 1, 9, 0, 0)));

        var values = await repository.GetMessagesAsync(-3);
        Assert.Equal([], values);
        values = await repository.GetMessagesAsync(0);
        Assert.Equal([], values);
    }
}