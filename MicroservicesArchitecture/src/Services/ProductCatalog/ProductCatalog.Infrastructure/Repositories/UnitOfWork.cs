using Microsoft.EntityFrameworkCore.Storage;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Repositories;

/// <summary>
/// Unit of work implementation
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ProductCatalogDbContext _context;
    private IDbContextTransaction? _transaction;
    private IProductRepository? _products;
    private ICategoryRepository? _categories;
    private ISupplierRepository? _suppliers;

    public UnitOfWork(ProductCatalogDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public ISupplierRepository Suppliers => _suppliers ??= new SupplierRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
