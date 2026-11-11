using backend.Enums;
using backend.Models;

namespace backend.Interfaces;

public interface IUserRepo
{
    Status Add(User user);
    User? GetUser(string username);
}
