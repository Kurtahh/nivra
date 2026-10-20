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

}
