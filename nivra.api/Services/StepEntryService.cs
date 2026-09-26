using backend.Models;
using backend.Repositories;

namespace backend.Services{
    public class StepEntryService
    {
        private readonly StepEntryRepo _repository;

        public StepEntryService(StepEntryRepo repository)
        {
            _repository = repository;
        }

        public void LogSteps(int stepCount)
        {
            if(stepCount < 1)
            {
                throw new ArgumentException("stepCount cannot be less than 1");
            }

            var entry = new StepEntry();

            entry.StepCount = stepCount;
            entry.Date = DateOnly.FromDateTime(DateTime.Now);
            entry.UserId = 1; //placeholder

            _repository.Add(entry);
        }
    }   
}