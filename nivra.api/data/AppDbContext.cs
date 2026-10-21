using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using backend.Enums;
using backend.Extensions;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<StepEntry> StepEntries { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<HealthTip> HealthTips { get; set; }
    
    public DbSet<Chat> Chat{ get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var categoryConverter = new ValueConverter<TipCategory, string>(
            category => category.ToString(),
            text => text.ToTipCategory());

        modelBuilder.Entity<HealthTip>()
            .Property(t=>t.Category)
            .HasConversion(categoryConverter);
    }
}