using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Repositories;

/// <summary>
/// Category repository implementation
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ProductCatalogDbContext _context;

    public CategoryRepository(ProductCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .Where(c => c.CategoryName.Contains(searchTerm) ||
                       (c.Description != null && c.Description.Contains(searchTerm)))
            .ToListAsync(cancellationToken);
    }

    public Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Add(category);
        return Task.FromResult(category);
    }

    public Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var category = await GetByIdAsync(id, cancellationToken);
        if (category != null)
        {
            _context.Categories.Remove(category);
        }
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AnyAsync(c => c.CategoryId == id, cancellationToken);
    }
}
