using System.Data;
using BuildingBlocks.Domain.Repositories;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

/// <summary>
/// DbContext for Identity module
/// </summary>
public class IdentityDbContext : DbContext, IUnitOfWork
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        // Set default schema
        modelBuilder.HasDefaultSchema("identity");
    }

    // IUnitOfWork implementation
    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    async Task<IDisposable> IUnitOfWork.BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    async Task<IDisposable> IUnitOfWork.BeginTransactionAsync(
        IsolationLevel isolationLevel,
        string? lockName,
        CancellationToken cancellationToken)
    {
        // For now, ignore lockName (can be implemented with distributed locks later)
        return await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    async Task IUnitOfWork.CommitTransactionAsync(CancellationToken cancellationToken)
    {
        await SaveChangesAsync(cancellationToken);
    }
}
