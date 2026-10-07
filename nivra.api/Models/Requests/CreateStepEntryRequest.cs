namespace backend.Models.Requests
{
    public record CreateStepEntryRequest{
        public int StepCount { get; set; }
        public long UserId { get; set; }
    }
}
