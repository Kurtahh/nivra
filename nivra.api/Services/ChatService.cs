using backend.Enums;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    
    public class ChatService
    {
        private readonly ChatRepo _repository;

        public ChatService(ChatRepo repository)
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
