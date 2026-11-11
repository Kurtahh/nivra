using backend.Enums;
using backend.Models;

namespace backend.Interfaces;

public interface IChatRepo
{
    SendStatus Add(Chat message);
    Task<List<Chat>> GetMessagesAsync(int count);
}
