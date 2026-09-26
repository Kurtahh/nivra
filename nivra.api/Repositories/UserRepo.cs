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
    } 
}
