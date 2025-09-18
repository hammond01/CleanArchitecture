using System.Linq.Expressions;

namespace CustomerManagement.Domain.Services;

public interface IRepository<T, TKey> where T : class
{
  IQueryable<T> GetQueryableSet();
  Task<List<T>> ToListAsync(IQueryable<T> query);
  Task<T?> FirstOrDefaultAsync(IQueryable<T> query);
  Task AddAsync(T entity, CancellationToken cancellationToken = default);
  Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
  void Delete(T entity);
  IUnitOfWork UnitOfWork { get; }
}
