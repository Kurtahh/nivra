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


        public Status SendMessageToChat(Chat message)
        {
            return _repository.Add(message);
        }
    }
}
