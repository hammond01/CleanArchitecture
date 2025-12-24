using System.Linq.Expressions;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Domain.Repositories;

/// <summary>
/// Generic repository interface with Specification Pattern support
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IRepository<TEntity, TKey> where TEntity : Entity<TKey>
{
    IUnitOfWork UnitOfWork { get; }

    // ===== Basic Query Methods (No Auto-Include) =====

    /// <summary>
    /// Get queryable set WITHOUT any automatic includes
    /// Use this when you want full control over the query
    /// </summary>
    IQueryable<TEntity> GetQueryableSet();

    /// <summary>
    /// Get queryable set WITH specific includes
    /// </summary>
    /// <param name="includes">Navigation properties to include</param>
    IQueryable<TEntity> GetQueryableSet(params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Get queryable set WITH string-based includes (for nested properties)
    /// Example: "Category.Parent"
    /// </summary>
    IQueryable<TEntity> GetQueryableSet(params string[] includeStrings);

    // ===== Specification Pattern Methods =====

    /// <summary>
    /// Get entity by specification
    /// </summary>
    Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of entities by specification
    /// </summary>
    Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count entities matching specification
    /// </summary>
    Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if any entity matches specification
    /// </summary>
    Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);

    // ===== CRUD Operations =====

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    void Delete(TEntity entity);

    // ===== Helper Methods (Backward Compatibility) =====

    Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query);

    Task<T?> SingleOrDefaultAsync<T>(IQueryable<T> query);

    Task<List<T>> ToListAsync<T>(IQueryable<T> query);
}

