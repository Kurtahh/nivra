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
        private void ValidateMinimumSteps(int minStepCount, int stepCount)
        {
            if(stepCount <= minStepCount)
            {
                throw new ArgumentException("Jūsų žingsnių kiekis privalo viršyti " + minStepCount.ToString());
            }
        }

        private void CreateAndSaveEntry(int stepCount, long userId, DateOnly date)
        {
            var entry = new StepEntry();

            entry.StepCount = stepCount;
            entry.Date = date;
            entry.UserId = userId;

            _repository.Add(entry);
        }

        private void OverrideEntrySteps(StepEntry entry, int newStepCount)
        {
            ValidateMinimumSteps(entry.StepCount, newStepCount);

            entry.StepCount = newStepCount;
            _repository.Update(entry);
        }

        public void LogSteps(int stepCount, long userId)
        {
            ValidateMinimumSteps(0, stepCount); // minimum is 0 (not including 0) 

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            StepEntry? oldStepEntry = _repository.EntryByDate(currentDate, userId);
            
            if(oldStepEntry == null)
                CreateAndSaveEntry(stepCount, userId, currentDate);
            else
                OverrideEntrySteps(oldStepEntry, stepCount);
        }
    }   
}