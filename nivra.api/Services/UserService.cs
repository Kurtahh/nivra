using System.Reflection.Metadata.Ecma335;
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
        private readonly IShopRepository<Order> _orderRepository;
        private readonly IShopRepository<Skin> _skinRepository;
        private readonly ISkinService _skinService;
        public UserService(
            IUserRepo repository, 
            IStepsRepo stepsRepository, 
            IShopRepository<Order> orderRepository,
            IShopRepository<Skin> skinRepository,
            ISkinService skinService
            )
        {
            _repository = repository;
            _stepsRepository = stepsRepository;
            _orderRepository = orderRepository;
            _skinRepository = skinRepository;
            _skinService = skinService;
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
            if(stepsToSum is not null)
                return stepsToSum.Sum(e => e.StepCount);
            else
                return 0;
        }

        public ProfileInfoResponse GetUserInfo(string username)
        {
            User user = _repository.GetUser(username);
            ProfileInfoResponse r = new ProfileInfoResponse();
            var entryToday = _stepsRepository.EntryByDate(DateOnly.FromDateTime(DateTime.UtcNow), user.Id);
            r.createdAt = user.CreatedAt;
            r.accountAge = Math.Max(0, (int)Math.Floor((DateTime.UtcNow - r.createdAt).TotalDays));
            if(entryToday is not null)
                r.stepsToday = entryToday.StepCount;
            else
                r.stepsToday = 0;
            r.stepsThisMonth = CalculateMonthSteps(user);
            r.equippedSkin = _skinService.GetEquippedSkinName(user.Id);

            return r;
        }
    }   
    
    
    
}