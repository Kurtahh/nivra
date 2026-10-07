using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class UserStepsOfToday{
        public required string Username { get; set; }
        public required int TodaySteps { get; set; }
    }
    
}
