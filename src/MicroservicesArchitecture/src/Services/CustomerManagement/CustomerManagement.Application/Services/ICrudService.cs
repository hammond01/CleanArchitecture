namespace CustomerManagement.Application.Services;

public interface ICrudService<T>
{
  Task<List<T>> GetAsync();
  Task<T?> GetByIdAsync(string id);
  Task AddAsync(T entity, CancellationToken cancellationToken = default);
  Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
  Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
