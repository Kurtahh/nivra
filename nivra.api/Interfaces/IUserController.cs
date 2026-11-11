using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces;

public interface IUserController
{
    IActionResult CreateUserOnSignUp(User user);
    IActionResult AuthenticateUser(User user);
    int GetTodaySteps(long id);
}
