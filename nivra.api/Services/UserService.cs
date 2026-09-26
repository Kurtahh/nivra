using backend.Models;
using backend.Repositories;

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
            _repository.Add(user);
        }
    }   
}