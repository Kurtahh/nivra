using backend.Models;

namespace backend.Repositories
{
    public class BlogRepo
    {
        private readonly AppDbContext _blogContext;

        public BlogRepo(AppDbContext blogContext)
        {
            _blogContext = blogContext;
        }

        public List<BlogPost> GetAll()
        {
            return _blogContext.BlogPosts
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }

        public User? GetUser(int userId)
        {
            return _blogContext.Users
                .FirstOrDefault(u => u.Id == userId);
        }

        public void Add(BlogPost post)
        {
            _blogContext.BlogPosts.Add(post);
            _blogContext.SaveChanges();
        }
        
        public bool Delete(int postId, int userId)
        {
            var post = _blogContext.BlogPosts
                .FirstOrDefault(p => p.Id == postId && p.UserId == userId);

            if (post == null)
                return false;

            _blogContext.BlogPosts.Remove(post);
            _blogContext.SaveChanges();

            return true;
        }

    }
}