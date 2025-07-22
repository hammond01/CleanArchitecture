using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly OrderManagementDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IOrderRepository? _orders;
    private bool _disposed = false;

    public UnitOfWork(OrderManagementDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IOrderRepository Orders
    {
        get
        {
            _orders ??= new OrderRepository(_context, _logger.CreateLogger<OrderRepository>());
            return _orders;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Saving changes to database");

        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Successfully saved {ChangeCount} changes to database", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Beginning database transaction");

        try
        {
            await _context.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error beginning database transaction");
            throw;
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Committing database transaction");

        try
        {
            await _context.CommitTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing database transaction");
            throw;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Rolling back database transaction");

        try
        {
            await _context.RollbackTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction rolled back successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back database transaction");
            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
