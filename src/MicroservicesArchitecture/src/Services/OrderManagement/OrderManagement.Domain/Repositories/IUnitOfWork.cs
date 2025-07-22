namespace OrderManagement.Domain.Repositories;

/// <summary>
/// Unit of Work interface
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Order repository
    /// </summary>
    IOrderRepository Orders { get; }

    /// <summary>
    /// Save all changes
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
