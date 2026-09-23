using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StepEntryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StepEntryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateStepEntry(StepEntry entry)
        {
            _context.Add(entry);
            _context.SaveChanges();
            return Ok();
        }
    }
}