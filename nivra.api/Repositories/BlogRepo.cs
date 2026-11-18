using backend.Models;

namespace backend.Repositories
{
    public class BlogRepo
    {
        private readonly AppDbContext _context;

        public BlogRepo(AppDbContext context)
        {
            _context = context;
        }

        public List<BlogPost> GetAll()
        {
            return _context.BlogPosts
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }

        public User? GetUser(int userId)
        {
            return _context.Users
                .FirstOrDefault(u => u.Id == userId);
        }

        public void Add(BlogPost post)
        {
            _context.BlogPosts.Add(post);
            _context.SaveChanges();
        }
    }
}