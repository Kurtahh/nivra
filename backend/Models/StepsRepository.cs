


namespace backend.Models
{
    public class StepsRepository
    {
        public readonly List<StepRecord> DailyRecords =
        [
            new StepRecord(new DateTime(2026, 09, 13), 5014, 15),
            new StepRecord(new DateTime(2026, 09, 14), 1000, 17),
            new StepRecord(new DateTime(2020, 3, 10), 34, 15),
            new StepRecord(new DateTime(2020, 12, 13), 6312, 100),
            new StepRecord(new DateTime(2020, 08, 13), 5514, 115),
            new StepRecord(new DateTime(2024, 11, 15), 4120, 150)
        ];


        public IEnumerable<StepRecord> GetAll()
        {
            var stepRecords =  DailyRecords.ToList();

            return stepRecords;
        }
        
        
        public IEnumerable<StepRecord> SearchByDate(string date)
        {
            var stepRecords =
                DailyRecords.Where(u => u.Date.Equals(DateTime.Parse(date)));

            return stepRecords;
        }
        


        
    }
}

