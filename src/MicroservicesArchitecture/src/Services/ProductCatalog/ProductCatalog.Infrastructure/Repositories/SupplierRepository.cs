using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Repositories;

/// <summary>
/// Supplier repository implementation
/// </summary>
public class SupplierRepository : ISupplierRepository
{
    private readonly ProductCatalogDbContext _context;

    public SupplierRepository(ProductCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.SupplierId == id, cancellationToken);
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .ToListAsync(cancellationToken);
    }

    public async Task<Supplier> AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Suppliers.AddAsync(supplier, cancellationToken);
        return entry.Entity;
    }

    public Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _context.Suppliers.Update(supplier);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var supplier = await _context.Suppliers.FindAsync(new object[] { id }, cancellationToken);
        if (supplier != null)
        {
            _context.Suppliers.Remove(supplier);
        }
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers.AnyAsync(s => s.SupplierId == id, cancellationToken);
    }
}
