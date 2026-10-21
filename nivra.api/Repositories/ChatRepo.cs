using backend.Enums;
using backend.Models;

namespace backend.Repositories
{
    public class ChatRepo 
    {
    
        private readonly AppDbContext _context;
        
        public ChatRepo(AppDbContext context)
        {
            _context = context;
        }
    
        public SendStatus Add(Chat message)
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

    
    }
}
