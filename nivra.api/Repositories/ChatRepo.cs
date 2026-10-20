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
    
        public Status Add(Chat message)
        {
            if (message.Message == "" || message.Message?.Length > 200)
            {
                return Status.Failure;
            }

            try
            {
                _context.Chat.Add(message);
            }
            catch (Exception e)
            {
                return Status.Failure;
            }
            return Status.Success;
        }

    
    }
}
