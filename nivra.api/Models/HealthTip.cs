using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("health_tips")]
    public record HealthTip(
        [property: Column("id")] int Id,
        [property: Column("content")] string Content,
        [property: Column("category")] TipCategory Category,
        [property: Column("author")] string? Author = null,
        [property: Column("source_url")] string? SourceUrl = null);
}