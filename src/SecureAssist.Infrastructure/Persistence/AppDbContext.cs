using Microsoft.EntityFrameworkCore;
using SecureAssist.Domain.Entities;

namespace SecureAssist.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // TODO: Implement tenant isolation enforcement
    }
}
