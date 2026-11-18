namespace backend.Models.Responses
{
    public record ProfileInfoResponse{
        public DateTime createdAt { get; set; }
        public int accountAge { get; set; }
        public int stepsToday { get; set; }
        public int stepsThisMonth { get; set; }
        public string? equippedSkin { get; set; }     
    }
}
