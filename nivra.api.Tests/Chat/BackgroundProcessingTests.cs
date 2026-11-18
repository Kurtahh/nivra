using backend.DataStructures;
using backend.Enums;
using backend.Interfaces;
using ChatMessage = backend.Models.Chat;
using backend.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace nivra.api.Tests.Chat;

public class BackgroundProcessingTests
{
    [Fact]
    public void Queue_ReturnsMessagesInTheOrderTheyWereEnqueued()
    {
        var queue = new BackgroundTaskQueue();
        queue.Enqueue(new ChatMessage { Message = "first" });
        queue.Enqueue(new ChatMessage { Message = "second" });

        Assert.True(queue.TryDequeue(out var first));
        Assert.True(queue.TryDequeue(out var second));
        Assert.False(queue.TryDequeue(out _));
        Assert.Equal("first", first.Message);
        Assert.Equal("second", second.Message);
    }

    [Fact]
    public void Queue_RejectsNullMessages()
    {
        var queue = new BackgroundTaskQueue();

        Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null!));
    }

    [Fact]
    public async Task HostedService_PersistsMessagesFromTheQueue()
    {
        var queue = new BackgroundTaskQueue();
        var message = new ChatMessage { Id = 5, Message = "hello", UserId = 1 };
        var processed = new TaskCompletionSource<ChatMessage>(TaskCreationOptions.RunContinuationsAsynchronously);

        var chatService = new Mock<IChatService>();
        chatService
            .Setup(service => service.SendMessageToChat(It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()))
            .Callback<ChatMessage, CancellationToken>((chat, _) => processed.TrySetResult(chat))
            .ReturnsAsync(SendStatus.Success);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(provider => provider.GetService(typeof(IChatService)))
            .Returns(chatService.Object);
        var scope = new Mock<IServiceScope>();
        scope.SetupGet(item => item.ServiceProvider).Returns(serviceProvider.Object);
        var scopeFactory = new Mock<IServiceScopeFactory>();
        scopeFactory.Setup(factory => factory.CreateScope()).Returns(scope.Object);

        var hostedService = new QueuedHostedService(
            queue,
            scopeFactory.Object,
            Mock.Of<IHubContext<ChatHub>>(),
            Mock.Of<ILogger<QueuedHostedService>>());

        await hostedService.StartAsync(CancellationToken.None);
        try
        {
            queue.Enqueue(message);

            var completed = await Task.WhenAny(processed.Task, Task.Delay(TimeSpan.FromSeconds(5)));

            Assert.Same(processed.Task, completed);
            Assert.Same(message, await processed.Task);
            chatService.Verify(
                service => service.SendMessageToChat(message, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            await hostedService.StopAsync(CancellationToken.None);
            hostedService.Dispose();
        }
    }
}
