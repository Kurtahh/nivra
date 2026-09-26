using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StepEntryController : ControllerBase
    {
        private readonly StepEntryService _service;

        public StepEntryController(StepEntryService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult CreateStepEntry([FromBody] int stepCount)
        {
            try
            {
                _service.LogSteps(stepCount); //repository pattern
                return Ok();
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}