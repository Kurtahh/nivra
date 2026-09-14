using backend.Models;
using Microsoft.AspNetCore.Mvc;


namespace backend.Controllers
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
        // get all
        [HttpGet]
        public ActionResult<List<StepRecord>> GetAllStepRecords() {
            var stepRecords = _stepsRepository.GetAll();
            return Ok(stepRecords);
        }
        // get by date
        [HttpGet]
        [Route("search")]
        public ActionResult<List<StepRecord>> SearchDates(string date)
        {
            var stepRecords = _stepsRepository.SearchByDate(date);
            if (stepRecords == null || !stepRecords.Any())
            {
                return NotFound();
            }

            return Ok(stepRecords);
        }
        

        
        

    }
}