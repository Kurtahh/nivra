using backend.Models;

namespace backend.Repositories
{
    public class StepsRepo
    {
        private readonly AppDbContext _context;
        
        public StepsRepo(AppDbContext context)
        {
            _context = context;
        }

        public StepEntry? EntryByDate(DateOnly date, long userId)
        {
            return _context.StepEntries.SingleOrDefault(e => e.UserId == userId && e.Date == date);
        }

        public void Add(StepEntry entry)
        {
            _context.Add(entry);
            _context.SaveChanges();
        }
        
        public void Update(StepEntry entry)
        {
            _context.StepEntries.Update(entry);
            _context.SaveChanges();
        }
    }
}