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
        return _dbContext.Model.FindEntityType(typeof(TEntity))
            ?.GetNavigations().Aggregate(query, func: (current, property) => current.Include
                (property.Name)) ?? query;
    }
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedDateTime = _dateTimeProvider.OffsetNow;
        await DbSet.AddAsync(entity, cancellationToken);
    }
    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedDateTime = _dateTimeProvider.OffsetNow;
        return Task.CompletedTask;
    }
    public void Delete(TEntity entity)
    {
        var entityToDelete = DbSet.Find(entity.Id);
        if (entityToDelete != null)
        {
            DbSet.Remove(entityToDelete);
        }
    }
    public Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query) => query.FirstOrDefaultAsync();
    public Task<T?> SingleOrDefaultAsync<T>(IQueryable<T> query) => query.SingleOrDefaultAsync();
    public Task<List<T>> ToListAsync<T>(IQueryable<T> query) => query.ToListAsync();
}
