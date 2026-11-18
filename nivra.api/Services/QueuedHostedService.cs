using System.Diagnostics;
using backend.Interfaces;
using backend.Models;
using backend.Enums;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace backend.Services;

public sealed class QueuedHostedService(
    IBackgroundTaskQueue queue,
    IServiceScopeFactory scopeFactory,
    IHubContext<ChatHub> hubContext,
    ILogger<QueuedHostedService> logger ) : BackgroundService
{
    
    private static readonly TimeSpan EmptyQueueDelay = TimeSpan.FromMilliseconds(200);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        logger.LogInformation("Queued hosted service started!");
        
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!queue.TryDequeue(out var message))
                {
                    await Task.Delay(EmptyQueueDelay, stoppingToken);
                    continue;
                }

                try
                {
                    using var scope = scopeFactory.CreateScope();


                    var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();
                    
                    logger.LogInformation(
                        "Processing message {MessageId}, {MessageText}",
                        message.Id,
                        message.Message);

                    SendStatus status = await chatService.SendMessageToChat(message, stoppingToken);
                    if (status != SendStatus.Success)
                    {
                        logger.LogWarning("Message {MessageId} was not saved: {Status}", message.Id, status);
                        continue;
                    }

                    logger.LogInformation("Message {MessageId} persisted", message.Id);

                    ChatMessageDto? savedMessage = await chatService.GetMessageByIdAsync(message.Id);
                    if (savedMessage is not null)
                    {
                        await hubContext.Clients.All.SendAsync("ReceiveMessage", savedMessage, stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation(
                        "Processing of message {MessageId} was cancelled during shutdown.",
                        message.Id);
                    break;
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "Failed to proccess the message {MessageId}",
                        message.Id);
                }

            }

        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // do normal shutdown
        }
        finally
        {
            logger.LogInformation("Queued hosted service stopped.");
        }
    }

    
    
}