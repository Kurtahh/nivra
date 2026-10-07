using backend.Models;

namespace backend.Repositories
{
    public class HealthTipRepo
    {
        private readonly AppDbContext _context;
        
        public HealthTipRepo(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<HealthTip> GetAll()
        {
            return _context.HealthTips;
        }

        public void Add(HealthTip tip)
        {
            _context.Add(tip);
            _context.SaveChanges();
        }
    }
}