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
    public IQueryable<TEntity> GetQueryableSet() => _dbContext.Set<TEntity>();
    public async Task AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity.Id != null && entity.Id.Equals(default(TKey)))
        {
            await AddAsync(entity, cancellationToken);
        }
        else
        {
            await UpdateAsync(entity, cancellationToken);
        }
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
    public void Delete(TEntity entity) => DbSet.Remove(entity);
    public Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query) => query.FirstOrDefaultAsync();
    public Task<T?> SingleOrDefaultAsync<T>(IQueryable<T> query) => query.SingleOrDefaultAsync();
    public Task<List<T>> ToListAsync<T>(IQueryable<T> query) => query.ToListAsync();
}
