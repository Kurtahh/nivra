using backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using backend.Models.Requests;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StepsController : ControllerBase, IStepsController
    {
        private readonly IStepsService _service;

        public StepsController(IStepsService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult CreateStepEntry([FromBody] CreateStepEntryRequest body)
        {
            try
            {
                _service.LogSteps(body.StepCount, body.UserId);
                return Ok();
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}