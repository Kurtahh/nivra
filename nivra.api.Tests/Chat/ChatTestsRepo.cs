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
        
        Assert.Equal(Status.Success, value);
        
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
        Assert.Equal(Status.Success, value);

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

        Assert.Equal(Status.Failure, value);
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

        Assert.Equal(Status.Failure, value);
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
        Assert.Equal(Status.Success, value);
        value = repository.Add(chatMessage2);
        Assert.Equal(Status.Failure, value);
        
    }
    
    
}