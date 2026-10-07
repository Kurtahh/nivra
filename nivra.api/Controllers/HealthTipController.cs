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

        [HttpGet]
        public IActionResult GetRandomTip()
        {
            try
            {
                var tip = _service.GetRandomTip();
                return Ok(tip);
            }
            catch(ArgumentException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}