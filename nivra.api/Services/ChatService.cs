using backend.Enums;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepo _repository;

        public ChatService(IChatRepo repository)
        {
            _repository = repository;
        }


        public async Task<SendStatus> SendMessageToChat(Chat message, CancellationToken stoppingToken)
        {
            message.DateT = DateTime.UtcNow;
            return await _repository.Add(message, stoppingToken);
        }

        public async Task<List<Chat>> GetMessagesAsync(int count = 50)
        {

            if (count > 100)
            {
                count = 100;
            } 
            return await _repository.GetMessagesAsync(count);
        }
    }
}
