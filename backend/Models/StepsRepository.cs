


namespace Nivra.Models
{
    public class StepsRepository
    {
        public List<StepRecord> DailyRecords = new List<StepRecord>() 
        {   
            new StepRecord(new DateTime(2026, 09, 13), 5014, 15),
            new StepRecord(new DateTime(2026, 09, 14), 1000, 17),
            new StepRecord(new DateTime(2026, 09, 13), 5014, 15),
            new StepRecord(new DateTime(2026, 09, 13), 5014, 15),
            new StepRecord(new DateTime(2026, 09, 13), 5014, 15),
            new StepRecord(new DateTime(2026, 09, 13), 5014, 15)
        };


        public IEnumerable<StepRecord> GetAll()
        {
            var stepRecords =  DailyRecords.ToList();

            return stepRecords;
        }


        
    }
}

