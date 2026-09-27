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
                
                HttpClient client = new HttpClient();
                var obj = new
                {
                    loginStatus = "Username doesn't exist.",
                };
                client.PostAsJsonAsync("https://localhost:5173/", obj);
                return Ok();
            }

            if (_service.CheckPassword(user, databaseUser))
            {
                HttpClient client = new HttpClient();
                var obj = new
                {
                    loginStatus = "Login successful",
                };
                client.PostAsJsonAsync("https://localhost:5173/", obj);
                return Ok();
            }
            else
            {
                HttpClient client = new HttpClient();
                var obj = new
                {
                    loginStatus = "Password is incorrect",
                };
                client.PostAsJsonAsync("https://localhost:5173/", obj);
                return Ok();
                
            }
            
            
            
            
        }
            
    }
}
