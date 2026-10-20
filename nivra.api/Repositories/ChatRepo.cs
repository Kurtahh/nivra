namespace backend.Repositories;

public class ChatRepo
{
    
    private readonly AppDbContext _context;
        
    public ChatRepo(AppDbContext context)
    {
        _context = context;
    }
    
}