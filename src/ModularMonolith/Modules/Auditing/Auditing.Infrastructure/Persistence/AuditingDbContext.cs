using Auditing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auditing.Infrastructure.Persistence;

/// <summary>
/// DbContext for Auditing module - audit log persistence
/// </summary>
public class AuditingDbContext : DbContext
{
    public AuditingDbContext(DbContextOptions<AuditingDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<AuditLogEntry> AuditLogEntries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditingDbContext).Assembly);

        // Set default schema
        modelBuilder.HasDefaultSchema("auditing");
    }
}
