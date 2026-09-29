using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;



namespace backend.Models
{
    public class User{
        
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public required string Username { get; set; }
        [Column("password_hash")]
        public required string PasswordHash { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
}
