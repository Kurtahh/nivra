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
    }
}
