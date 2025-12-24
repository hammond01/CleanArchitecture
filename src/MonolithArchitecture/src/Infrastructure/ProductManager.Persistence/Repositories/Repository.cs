using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Shared.DateTimes;

namespace ProductManager.Persistence.Repositories;

/// <summary>
/// Generic repository implementation with Specification Pattern support
/// </summary>
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : Entity<TKey>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ApplicationDbContext _dbContext;

    public Repository(IDateTimeProvider dateTimeProvider, ApplicationDbContext dbContext)
    {
        _dateTimeProvider = dateTimeProvider;
        _dbContext = dbContext;
    }

    private DbSet<TEntity> DbSet => _dbContext.Set<TEntity>();
    public IUnitOfWork UnitOfWork => _dbContext;

    // ===== Basic Query Methods (No Auto-Include) =====

    /// <summary>
    /// Get queryable set WITHOUT any automatic includes
    /// ✅ FIXED: No more auto-include performance issue!
    /// </summary>
    public IQueryable<TEntity> GetQueryableSet()
    {
        return _dbContext.Set<TEntity>().AsQueryable();
    }

    /// <summary>
    /// Get queryable set WITH specific includes
    /// </summary>
    public IQueryable<TEntity> GetQueryableSet(params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }

    /// <summary>
    /// Get queryable set WITH string-based includes (for nested properties)
    /// </summary>
    public IQueryable<TEntity> GetQueryableSet(params string[] includeStrings)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        foreach (var includeString in includeStrings)
        {
            query = query.Include(includeString);
        }

        return query;
    }

    // ===== Specification Pattern Methods =====

    /// <summary>
    /// Get single entity by specification
    /// </summary>
    public async Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Get list of entities by specification
    /// </summary>
    public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec);
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Count entities matching specification
    /// </summary>
    public async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec);
        return await query.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Check if any entity matches specification
    /// </summary>
    public async Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec);
        return await query.AnyAsync(cancellationToken);
    }

    // ===== CRUD Operations =====

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedDateTime = _dateTimeProvider.OffsetNow;
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedDateTime = _dateTimeProvider.OffsetNow;

        // For relational databases, ensure the entity is marked as modified
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }
        _dbContext.Entry(entity).State = EntityState.Modified;

        return Task.CompletedTask;
    }

    public void Delete(TEntity entity)
    {
        // For relational databases, we need to attach it first if it's not tracked
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }
        DbSet.Remove(entity);
    }

    // ===== Helper Methods (Backward Compatibility) =====

    public Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query) => query.FirstOrDefaultAsync();
    public Task<T?> SingleOrDefaultAsync<T>(IQueryable<T> query) => query.SingleOrDefaultAsync();
    public Task<List<T>> ToListAsync<T>(IQueryable<T> query) => query.ToListAsync();

    // ===== Private Helper Methods =====

    /// <summary>
    /// Apply specification to queryable
    /// </summary>
    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        return SpecificationEvaluator<TEntity>.GetQuery(_dbContext.Set<TEntity>().AsQueryable(), spec);
    }
}

