using backend.Enums;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        private readonly StepsService _stepsService;

        public UserController(UserService service, StepsService serviceStepEntry)
        {
            _service = service;
            _stepsService = serviceStepEntry;
        }

        [HttpPost]
        public IActionResult CreateUserOnSignUp(User user)
        {


            if (_service.SignUpUser(user) == Status.Failure)
            {
                return Conflict(new
                {
                   message = "Username already exist or you entered not secure password!"
                });
            }
            return Ok("Sign up was successful");
        }

        [HttpPost]
        [Route("Authenticate")]
        public IActionResult AuthenticateUser(User user)
        {
            var databaseUser = _service.CheckIfUsernameExists(user.Username);
            if (databaseUser == null )
            {       
                Console.WriteLine("Username doesn't exist!");
                return Ok(new [] {"Username doesn't exist!", "false"});
            }

            if (_service.CheckPassword(user, databaseUser))
            {
                Console.WriteLine("Login successful!");
                return Ok(new [] {"Login successful!", "true", databaseUser.Id.ToString()});
            }

            Console.WriteLine("Password is not correct!");
            return Ok(new [] {"Password is not correct!", "false"});
        }


        [HttpGet]
        [Route("TodaySteps/{id:long}")]
        public int GetTodaySteps(long id)
        {
            return _stepsService.GetTodaySteps(id);
        }
    }
}
