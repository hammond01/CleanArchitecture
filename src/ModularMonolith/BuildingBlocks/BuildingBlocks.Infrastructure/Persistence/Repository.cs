using System.Linq.Expressions;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Repositories;
using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Generic repository implementation with Specification Pattern support
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> 
    where TEntity : Entity<TKey>
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
    }

    public IUnitOfWork UnitOfWork => (IUnitOfWork)Context;

    // ===== Basic Query Methods =====

    public IQueryable<TEntity> GetQueryableSet()
    {
        return DbSet.AsQueryable();
    }

    public IQueryable<TEntity> GetQueryableSet(params Expression<Func<TEntity, object>>[] includes)
    {
        var query = DbSet.AsQueryable();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return query;
    }

    public IQueryable<TEntity> GetQueryableSet(params string[] includeStrings)
    {
        var query = DbSet.AsQueryable();
        
        foreach (var includeString in includeStrings)
        {
            query = query.Include(includeString);
        }
        
        return query;
    }

    // ===== Specification Pattern Methods =====

    public async Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).CountAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).AnyAsync(cancellationToken);
    }

    // ===== CRUD Operations =====

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedDateTime = DateTimeOffset.UtcNow;
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedDateTime = DateTimeOffset.UtcNow;
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    // ===== Helper Methods =====

    public async Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query)
    {
        return await query.FirstOrDefaultAsync();
    }

    public async Task<T?> SingleOrDefaultAsync<T>(IQueryable<T> query)
    {
        return await query.SingleOrDefaultAsync();
    }

    public async Task<List<T>> ToListAsync<T>(IQueryable<T> query)
    {
        return await query.ToListAsync();
    }

    // ===== Private Methods =====

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        return SpecificationEvaluator<TEntity>.GetQuery(DbSet.AsQueryable(), spec);
    }
}
