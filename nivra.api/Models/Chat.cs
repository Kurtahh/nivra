namespace backend.Models
{
    public class Chat{
        public int Id { get; set; }
        public DateTime DateT { get; set; }
        public string? Message { get; set; }
        public long UserId { get; set; }
    }
}