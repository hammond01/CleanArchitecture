using BuildingBlocks.Application.Auditing;
using BuildingBlocks.Application.Dispatcher;
using BuildingBlocks.Application.Security;
using BuildingBlocks.Infrastructure.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

/// <summary>
/// DbContext for Identity module
/// </summary>
public class IdentityDbContext : ModuleDbContextBase
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : this(options, null, null, null)
    {
    }

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        IDispatcher? dispatcher,
        IEntityChangeBuffer? entityChangeBuffer,
        ICurrentUserAccessor? currentUserAccessor)
        : base(options, dispatcher, entityChangeBuffer, currentUserAccessor)
    {
    }

    protected override string ModuleName => "Identity";

    // DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<AuditOutboxMessage> AuditOutboxMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        // Set default schema
        modelBuilder.HasDefaultSchema("identity");
    }
}
