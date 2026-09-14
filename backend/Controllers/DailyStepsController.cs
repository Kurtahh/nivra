using Nivra.Models;
using Microsoft.AspNetCore.Mvc;


namespace Nivra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyStepsController : ControllerBase
    {
        private readonly StepsRepository _stepsRepository;

        public DailyStepsController(StepsRepository stepsRepository)
        {
            _stepsRepository = stepsRepository;
        }


        [HttpGet]
        public ActionResult<List<StepRecord>> GetAllStepRecords() {
            var step_records = _stepsRepository.GetAll();
            return Ok(step_records);
        }

    }
}