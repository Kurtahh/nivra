using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("chat")]
    public class Chat{
        public int Id { get; set; }
        public DateTime DateT { get; set; }
        public string? Message { get; set; }
        public long UserId { get; set; }
    }
}