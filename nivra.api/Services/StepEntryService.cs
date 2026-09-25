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

        public void LogSteps(StepEntry entry)
        {
            _repository.Add(entry);
        }
    }   
}