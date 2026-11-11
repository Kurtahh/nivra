using backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using backend.Exceptions;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthTipController : ControllerBase, IHealthTipController
    {
        private readonly IHealthTipService _service;

        public HealthTipController(IHealthTipService service)
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
            catch (InvalidTipCategoryException e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}