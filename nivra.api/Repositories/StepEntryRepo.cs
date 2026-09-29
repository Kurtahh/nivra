using backend.Models;

namespace backend.Repositories
{
    public class StepEntryRepo
    {
        private readonly AppDbContext _context;
        
        public StepEntryRepo(AppDbContext context)
        {
            _context = context;
        }

        public void Add(StepEntry entry)
        {
            _context.Add(entry);
            _context.SaveChanges();
        }
    }
}