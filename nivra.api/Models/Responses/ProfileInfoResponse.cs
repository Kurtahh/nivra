namespace backend.Models.Responses
{
    public record ProfileInfoResponse{
        public DateTime createdAt { get; set; }
        public int accountAge { get; set; }
        public int stepsThisMonth { get; set; }        
    }
}
