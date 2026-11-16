using backend.Enums;
using backend.Models;

namespace backend.Interfaces;

public interface IChatRepo
{
    Task<SendStatus> Add(Chat message, CancellationToken stoppingToken);
    Task<List<Chat>> GetMessagesAsync(int count);
}
