using BuildingBlocks.Application.Auditing;
using BuildingBlocks.Application.Dispatcher;
using BuildingBlocks.Application.Security;
using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public class CatalogDbContext : ModuleDbContextBase
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : this(options, null, null, null)
    {
    }

    public CatalogDbContext(
        DbContextOptions<CatalogDbContext> options,
        IDispatcher? dispatcher,
        IEntityChangeBuffer? entityChangeBuffer,
        ICurrentUserAccessor? currentUserAccessor)
        : base(options, dispatcher, entityChangeBuffer, currentUserAccessor)
    {
    }

    protected override string ModuleName => "Catalog";

    // DbSets
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<AuditOutboxMessage> AuditOutboxMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        
        // Set default schema (optional)
        modelBuilder.HasDefaultSchema("catalog");
    }
}
