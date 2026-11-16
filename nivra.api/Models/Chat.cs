using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("chat")]
    public class Chat{
        [Column ("id")]
        public int Id { get; set; }
        [Column ("created_at")]
        public DateTime DateT { get; set; }
        [Column ("message")]
        public string? Message { get; set; }
        [Column ("user_id")]
        public long UserId { get; set; }
    }
}