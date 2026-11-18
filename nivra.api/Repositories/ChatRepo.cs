using System.Data;
using System.Reflection.Metadata;
using backend.Enums;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class ChatRepo : IChatRepo
    {
    
        private readonly AppDbContext _context;
        
        public ChatRepo(AppDbContext context)
        {
            _context = context;
        }
    
        public async Task<SendStatus> Add(Chat message, CancellationToken stoppingToken)
        {
            
            
            if (string.IsNullOrWhiteSpace(message.Message)) 
            {
                return SendStatus.MessageIsEmpty;
            }

            if (message.Message?.Length > 200)
            {
                return SendStatus.MessageAbove200;
            }
            
            try
            {
                _context.Chat.Add(message);
                _context.SaveChanges();
            }
            catch 
            {
                return SendStatus.MessageAlreadyExists;
            }
            
            return SendStatus.Success;
        }

        public async Task<List<ChatMessageDto>> GetMessagesAsync(int count)
        {

            if (count <= 0) return [];

            return await _context.Chat
                .AsNoTracking()
                .Join(
                    _context.Users,
                    chat => chat.UserId,
                    user => (long)user.Id,
                    (chat, user) => new { Chat = chat, User = user })
                .OrderByDescending(row => row.Chat.DateT)
                .ThenByDescending(row => row.Chat.Id)
                .Take(count)
                .OrderBy(row => row.Chat.DateT)
                .ThenBy(row => row.Chat.Id)
                .Select(row => new ChatMessageDto(
                    row.Chat.Id,
                    row.Chat.DateT,
                    row.Chat.Message ?? "",
                    row.Chat.UserId,
                    row.User.Username,
                    ""
                ))
                .ToListAsync();
        }

        public async Task<ChatMessageDto?> GetMessageByIdAsync(int id)
        {
            return await _context.Chat
                .AsNoTracking()
                .Where(chat => chat.Id == id)
                .Join(
                    _context.Users,
                    chat => chat.UserId,
                    user => (long)user.Id,
                    (chat, user) => new { Chat = chat, User = user })
                .Select(row => new ChatMessageDto(
                    row.Chat.Id,
                    row.Chat.DateT,
                    row.Chat.Message ?? "",
                    row.Chat.UserId,
                    row.User.Username,
                    ""
                ))
                .FirstOrDefaultAsync();
        }
    }
}
