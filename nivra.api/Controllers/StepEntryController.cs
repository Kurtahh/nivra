using backend.Models;
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
        public IActionResult CreateStepEntry(StepEntry entry)
        {
            _service.LogSteps(entry); //repository pattern
            return Ok();
        }
    }
}