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

        var value = service.SendMessageToChat(chatMessage);
        Assert.Equal(Status.Success, value);

    }
}