using backend.Enums;
using backend.Models;

namespace backend.Interfaces;

public interface IChatService
{
    Task<SendStatus> SendMessageToChat(Chat message, CancellationToken stoppingToken);
    Task<List<Chat>> GetMessagesAsync(int count = 50);
}
