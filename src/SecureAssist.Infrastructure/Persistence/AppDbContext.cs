using Microsoft.EntityFrameworkCore;
using SecureAssist.Domain.Entities;

namespace SecureAssist.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<AIInteraction> AIInteractions { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<SearchLog> SearchLogs { get; set; }
    public DbSet<WorkflowActionRecord> WorkflowActionRecords { get; set; }

    // Mocking a tenant ID for the session. In production, this comes from a service.
    public Guid CurrentTenantId { get; set; } = Guid.Empty;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // SECURE: Global Query Filter for Tenant Isolation
        modelBuilder.Entity<Document>().HasQueryFilter(e => e.TenantId == CurrentTenantId);
        modelBuilder.Entity<AIInteraction>().HasQueryFilter(e => e.TenantId == CurrentTenantId);
    }
}
