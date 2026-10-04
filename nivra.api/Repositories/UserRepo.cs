using backend.Enums;
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


        public Status Add(User user)
        {
            var databaseUser = GetUser(user.Username);
            if (databaseUser == null && user.Username != "" && user.PasswordHash != "")
            {
                
                _context.Add(user);
                _context.SaveChanges();
                return Status.Success;
            }

            return Status.Failure;
        }


        public User? GetUser(string username)
        {
             return _context.Users.SingleOrDefault(u => u.Username == username);
        }
    } 
}
