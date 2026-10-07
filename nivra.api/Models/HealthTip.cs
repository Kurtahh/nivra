namespace backend.Models
{
    public record HealthTip(int Id, string Content, TipCategory Category, string? Author = null, string? SourceUrl = null);
}