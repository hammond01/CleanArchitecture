using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Data.Configurations;
using Shared.Common.Domain;

namespace OrderManagement.Infrastructure.Data;

/// <summary>
/// Order Management database context
/// </summary>
public class OrderManagementDbContext : DbContext
{
    private IDbContextTransaction? _currentTransaction;

    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new MoneyConfiguration());
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerInfoConfiguration());

        // Configure schema
        modelBuilder.HasDefaultSchema("OrderManagement");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps before saving
        UpdateTimestamps();

        // Dispatch domain events before saving
        await DispatchDomainEventsAsync();

        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return _currentTransaction;
        }

        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _currentTransaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // CreatedAt is set in the constructor
                    break;
                case EntityState.Modified:
                    entry.Property(e => e.UpdatedAt).CurrentValue = DateTime.UtcNow;
                    break;
            }
        }
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        // TODO: Publish domain events using MediatR or event bus
        // For now, we'll just log them
        foreach (var domainEvent in domainEvents)
        {
            // Log domain event
            Console.WriteLine($"Domain Event: {domainEvent.GetType().Name} - {domainEvent.Id}");
        }

        await Task.CompletedTask;
    }
}
