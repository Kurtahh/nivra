using System.Diagnostics;
using backend.Interfaces;
using backend.Models;

namespace backend.Services;

public sealed class QueuedHostedService(
    IBackgroundTaskQueue queue,
    IServiceScopeFactory scopeFactory,
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

                    await chatService.SendMessageToChat(message, stoppingToken);
                    logger.LogInformation("Message {MessageId} persisted", message.Id);
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