using BuildingBlocks.Domain.Repositories;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public class CatalogDbContext : DbContext, IUnitOfWork
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        
        // Set default schema (optional)
        modelBuilder.HasDefaultSchema("catalog");
    }

    // IUnitOfWork implementation
    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    async Task<IDisposable> IUnitOfWork.BeginTransactionAsync(
        System.Data.IsolationLevel isolationLevel,
        CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    async Task<IDisposable> IUnitOfWork.BeginTransactionAsync(
        System.Data.IsolationLevel isolationLevel,
        string? lockName,
        CancellationToken cancellationToken)
    {
        // For now, ignore lockName (can be implemented with distributed locks later)
        return await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    async Task IUnitOfWork.CommitTransactionAsync(CancellationToken cancellationToken)
    {
        var transaction = Database.CurrentTransaction;
        if (transaction == null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        try
        {
            await SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
