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
        // get by date && optional id
        [HttpGet]
        [Route("search")]
        public ActionResult<List<StepRecord>> SearchDates([FromQuery] string date, [FromQuery] int? id)
        {
            var stepRecords = _stepsRepository.SearchByDate(date);


            if (id != null)
                stepRecords = stepRecords.Where(u => u.UserId.Equals(id));
            
            if (!stepRecords.Any())
            {
                return NotFound("No employees match the criteria");
            }

            return Ok(stepRecords);
        }
        

        
        

    }
}