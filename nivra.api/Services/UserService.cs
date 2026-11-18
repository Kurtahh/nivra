using backend.Enums;
using backend.Interfaces;
using backend.Models;
using backend.Models.Responses;
using Microsoft.AspNetCore.Identity;

namespace backend.Services{
    public class  UserService : IUserService
    {
        private readonly IUserRepo _repository;
        private readonly IStepsRepo _stepsRepository;

        public UserService(IUserRepo repository, IStepsRepo stepsRepository)
        {
            _repository = repository;
            _stepsRepository = stepsRepository;
        }

        public Status SignUpUser(User user)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, user.PasswordHash);
            return _repository.Add(user);
        }

        public User? CheckIfUsernameExists(string username)
        {
            return _repository.GetUserNullable(username);

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


        public User? GetUserByName(string name)
        {
            return _repository.GetUserNullable(name);
        }

        private int CalculateMonthSteps(User u)
        {
            DateOnly oneMonthAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1).Date);
            var stepsToSum = _stepsRepository.EntriesAfterDate(oneMonthAgo, u.Id);
            return stepsToSum.Sum(e => e.StepCount);
        }

        public ProfileInfoResponse GetUserInfo(string username)
        {
            User user = _repository.GetUser(username);
            ProfileInfoResponse r = new ProfileInfoResponse();
            r.createdAt = user.CreatedAt;
            r.accountAge = Math.Max(0, (int)Math.Floor((DateTime.UtcNow - r.createdAt).TotalDays));
            r.stepsThisMonth = CalculateMonthSteps(user);

            return r;
        }
    }   
    
    
    
}