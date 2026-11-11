using backend.Hubs;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using ChatMessage = backend.Models.Chat;

namespace nivra.api.Tests.Chat;

public class ChatHubTests
{
    [Fact]
    public async Task SendMessage_PersistsAndBroadcastsToAllClients()
    {
        var (hub, context, clientProxy) = CreateHub();
        var message = new ChatMessage
        {
            Message = "Hello my health-nuts",
            UserId = 42
        };

        await hub.SendMessage(message);

        var saved = Assert.Single(context.Chat);
        Assert.Equal("Hello my health-nuts", saved.Message);
        Assert.Equal(42, saved.UserId);
        Assert.True(message.Id > 0, "The persisted id should be sent to clients");

        clientProxy.Verify(proxy => proxy.SendCoreAsync(
            "ReceiveMessage",
            It.Is<object?[]>(args => args.Length == 1 && args[0] == message),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    public async Task SendMessage_RejectsEmptyMessagesWithoutBroadcasting(string text)
    {
        var (hub, context, clientProxy) = CreateHub();

        var exception = await Assert.ThrowsAsync<HubException>(
            () => hub.SendMessage(new ChatMessage { Message = text, UserId = 1 }));

        Assert.Equal("chat:Message is empty", exception.Message);
        Assert.Empty(context.Chat);
        VerifyNoBroadcast(clientProxy);
    }

    [Fact]
    public async Task SendMessage_RejectsMessagesLongerThan200Characters()
    {
        var (hub, context, clientProxy) = CreateHub();

        var exception = await Assert.ThrowsAsync<HubException>(
            () => hub.SendMessage(new ChatMessage
            {
                Message = new string('F', 201),
                UserId = 1
            }));

        Assert.Equal("chat:Message is above 200!", exception.Message);
        Assert.Empty(context.Chat);
        VerifyNoBroadcast(clientProxy);
    }

    private static void VerifyNoBroadcast(Mock<IClientProxy> clientProxy)
    {
        clientProxy.Verify(proxy => proxy.SendCoreAsync(
            It.IsAny<string>(),
            It.IsAny<object?[]>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    private static (MessageNotificationHub Hub, AppDbContext Context, Mock<IClientProxy> ClientProxy) CreateHub()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var service = new ChatService(new ChatRepo(context));

        var clientProxy = new Mock<IClientProxy>();
        clientProxy
            .Setup(proxy => proxy.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var clients = new Mock<IHubCallerClients>();
        clients.Setup(c => c.All).Returns(clientProxy.Object);

        var hub = new MessageNotificationHub(service) { Clients = clients.Object };

        return (hub, context, clientProxy);
    }
}
