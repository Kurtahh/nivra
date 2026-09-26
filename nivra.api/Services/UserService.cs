using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Identity;

namespace backend.Services{
    public class  UserService 
    {
        private readonly UserRepo _repository;

        public UserService(UserRepo repository)
        {
            _repository = repository;
        }

        public void SignUpUser(User user)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, user.PasswordHash);
            _repository.Add(user);
        }
    }   
}