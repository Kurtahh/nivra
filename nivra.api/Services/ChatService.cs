using backend.Enums;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepo _repository;
        private readonly ISkinService _skinService;

        public ChatService(IChatRepo repository, ISkinService skinService)
        {
            _repository = repository;
            _skinService = skinService;
        }


        public async Task<SendStatus> SendMessageToChat(Chat message, CancellationToken stoppingToken)
        {
            message.DateT = DateTime.UtcNow;
            return await _repository.Add(message, stoppingToken);
        }

        public async Task<List<ChatMessageDto>> GetMessagesAsync(int count = 50)
        {

            if (count > 100)
            {
                count = 100;
            }

            List<ChatMessageDto> messages = await _repository.GetMessagesAsync(count);

            Dictionary<long, string> skins = messages
                .Select(m => m.UserId)
                .Distinct()
                .ToDictionary(id => id, id => _skinService.GetEquippedSkinName(id));

            return messages
                .Select(m => m with { equippedSkin = skins[m.UserId] })
                .ToList();
        }
    }
}