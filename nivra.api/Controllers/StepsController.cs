using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StepsController : ControllerBase
    {
        private readonly StepsService _service;

        public StepsController(StepsService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult CreateStepEntry([FromBody] int stepCount)
        {
            try
            {
                _service.LogSteps(stepCount, 18); // 18 - placeholder userId
                return Ok();
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}