using System.Linq.Expressions;
using CustomerManagement.Domain.Services;
using CustomerManagement.Domain.Entities;
using CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Infrastructure.Repositories;

public class Repository<T, TKey> : IRepository<T, TKey>
    where T : Entity<TKey>
{
    protected readonly CustomerManagementDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(CustomerManagementDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> GetQueryableSet() => _dbSet.AsQueryable();

    public async Task<List<T>> ToListAsync(IQueryable<T> query)
        => await query.ToListAsync();

    public async Task<T?> FirstOrDefaultAsync(IQueryable<T> query)
        => await query.FirstOrDefaultAsync();

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public void Delete(T entity)
        => _dbSet.Remove(entity);

    public IUnitOfWork UnitOfWork => new UnitOfWork(_context);
}
