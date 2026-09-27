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

        public UserController(UserService service)
        {
            _service = service;
        }


        [HttpPost]
        public IActionResult CreateUserOnSignUp(User user)
        {
            _service.SignUpUser(user);
            return Ok();
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
            
    }
}
