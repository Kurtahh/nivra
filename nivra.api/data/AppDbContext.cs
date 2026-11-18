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
    public DbSet<BlogPost> BlogPosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var categoryConverter = new ValueConverter<TipCategory, string>(
            category => category.ToString(),
            text => text.ToTipCategory());

        modelBuilder.Entity<HealthTip>()
            .Property(t=>t.Category)
            .HasConversion(categoryConverter);
        
        var blogCategoryConverter = new ValueConverter<BlogCategory, string>(
            category => category.ToString(),
            text => Enum.Parse<BlogCategory>(text));

        modelBuilder.Entity<BlogPost>()
            .Property(p => p.Category)
            .HasConversion(blogCategoryConverter);
    }
}