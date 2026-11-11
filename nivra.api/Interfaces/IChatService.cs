using backend.Enums;
using backend.Models;

namespace backend.Interfaces;

public interface IChatService
{
    SendStatus SendMessageToChat(Chat message);
    Task<List<Chat>> GetMessagesAsync(int count = 50);
}
