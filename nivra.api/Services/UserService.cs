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

        public User? CheckIfUsernameExists(string username)
        {
            return _repository.GetUser(username);

        }

        public bool CheckPassword(User inputUser, User databaseUser)
        {
             var hasher = new PasswordHasher<User>();
             var result = hasher.VerifyHashedPassword(inputUser, databaseUser.PasswordHash, inputUser.PasswordHash);
             if (result == PasswordVerificationResult.Success)
             {
                 
                 return true;
             }
             
             return false;
        }
    }   
    
    
    
}