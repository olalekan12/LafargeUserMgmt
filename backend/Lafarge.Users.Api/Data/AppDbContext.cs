using Lafarge.Users.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lafarge.Users.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.HasIndex(u => new { u.FirstName, u.LastName, u.Email, u.Phone }).HasDatabaseName("IX_Users_NameEmailPhone");
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.Role).HasConversion<string>().HasMaxLength(16);
        });
    }
}
