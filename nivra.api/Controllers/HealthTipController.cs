using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthTipController : ControllerBase
    {
        private readonly HealthTipService _service;

        public HealthTipController(HealthTipService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult GetTip()
        {
            try
            {
                _service.GetRandomTip();
                return Ok();
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}