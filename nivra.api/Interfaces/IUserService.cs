using backend.Enums;
using backend.Models;

namespace backend.Interfaces;

public interface IUserService
{
    Status SignUpUser(User user);
    User? CheckIfUsernameExists(string username);
    bool CheckPassword(User inputUser, User databaseUser);
    User? GetUserByName(string name);
}
