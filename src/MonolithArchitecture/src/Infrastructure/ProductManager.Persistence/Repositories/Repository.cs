using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Shared.DateTimes;
namespace ProductManager.Persistence.Repositories;

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
    public IQueryable<TEntity> GetQueryableSet()
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        // Use model metadata to include navigation properties
        var entityType = _dbContext.Model.FindEntityType(typeof(TEntity));
        if (entityType != null)
        {
            var navigationProperties = entityType.GetNavigations();
            foreach (var navigation in navigationProperties)
            {
                query = query.Include(navigation.Name);
            }
        }

        return query;
    }
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
    public Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query) => query.FirstOrDefaultAsync();
    public Task<T?> SingleOrDefaultAsync<T>(IQueryable<T> query) => query.SingleOrDefaultAsync();
    public Task<List<T>> ToListAsync<T>(IQueryable<T> query) => query.ToListAsync();
}
