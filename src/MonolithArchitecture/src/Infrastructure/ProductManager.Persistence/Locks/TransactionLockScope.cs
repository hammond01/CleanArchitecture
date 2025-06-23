using Microsoft.EntityFrameworkCore.Storage;
using ProductManager.Shared.Locks;

namespace ProductManager.Persistence.Locks;

/// <summary>
/// Combines database transaction with distributed locking for atomic operations
/// </summary>
public class TransactionLockScope : IDisposable
{
    private readonly IDbContextTransaction _transaction;
    private readonly ILockManager _lockManager;
    private readonly string _entityName;
    private readonly string _entityId;
    private readonly string _ownerId;
    private bool _lockAcquired;
    private bool _disposed;

    public TransactionLockScope(IDbContextTransaction transaction, ILockManager lockManager,
        string entityName, string entityId, string ownerId)
    {
        _transaction = transaction;
        _lockManager = lockManager;
        _entityName = entityName;
        _entityId = entityId;
        _ownerId = ownerId;
    }

    /// <summary>
    /// Acquire the distributed lock
    /// </summary>
    public bool AcquireLock(TimeSpan timeout)
    {
        _lockAcquired = _lockManager.AcquireLock(_entityName, _entityId, _ownerId, timeout);
        return _lockAcquired;
    }

    /// <summary>
    /// Commit both the database transaction and release the lock
    /// </summary>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(TransactionLockScope));

        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            ReleaseLock();
        }
    }

    /// <summary>
    /// Rollback the transaction and release the lock
    /// </summary>
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(TransactionLockScope));

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            ReleaseLock();
        }
    }

    private void ReleaseLock()
    {
        if (_lockAcquired)
        {
            _lockManager.ReleaseLock(_entityName, _entityId, _ownerId);
            _lockAcquired = false;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            ReleaseLock();
            _transaction.Dispose();
            _disposed = true;
        }
    }
}
