using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<StepEntry> StepEntries { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<HealthTip> HealthTips { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var categoryConverter = new ValueConverter<TipCategory, string>(
            category => category.ToString(),
            text => Enum.Parse<TipCategory>(text, ignoreCase: true));

        modelBuilder.Entity<HealthTip>()
            .Property(t=>t.Category)
            .HasConversion(categoryConverter);
    }
}