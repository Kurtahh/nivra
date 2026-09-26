using backend.Models;


namespace backend.Repositories
{
    public class UserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }


        public void Add(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
        }


        public User? GetUser(string username)
        {
             return _context.Users.SingleOrDefault(u => u.Username == username);
        }
    } 
}
