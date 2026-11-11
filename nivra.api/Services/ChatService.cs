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


        public SendStatus SendMessageToChat(Chat message)
        {
            message.DateT = DateTime.UtcNow;
            return _repository.Add(message);
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
