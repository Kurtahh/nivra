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
            HttpClient client = new HttpClient();
            var obj = new
            {
                loginStatus = "Login was successful",
            };
            client.PostAsJsonAsync("https://localhost:5173/", obj);
            return Ok();
        }
            
    }
}
