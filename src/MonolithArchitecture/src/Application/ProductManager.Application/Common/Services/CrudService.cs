using ProductManager.Domain.Entities;
using ProductManager.Domain.Events;
using ProductManager.Domain.Repositories;
using ProductManager.Shared.Exceptions;
namespace ProductManager.Application.Common.Services;

public class CrudService<T> : ICrudService<T>
    where T : Entity<string>
{
    private readonly Dispatcher _dispatcher;
    private readonly IRepository<T, string> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CrudService(IRepository<T, string> repository, Dispatcher dispatcher)
    {
        _unitOfWork = repository.UnitOfWork;
        _repository = repository;
        _dispatcher = dispatcher;
    }

    public Task<List<T>> GetAsync()
        => _repository.ToListAsync(_repository.GetQueryableSet());

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _dispatcher.DispatchAsync(new EntityCreatedEvent<T>(entity, DateTimeOffset.UtcNow), cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _dispatcher.DispatchAsync(new EntityUpdatedEvent<T>(entity, DateTimeOffset.UtcNow), cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _repository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _dispatcher.DispatchAsync(new EntityDeletedEvent<T>(entity, DateTimeOffset.UtcNow), cancellationToken);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        ValidationException.Requires(!string.IsNullOrEmpty(id), "Invalid Id");
        return await _repository.FirstOrDefaultAsync(_repository.GetQueryableSet().Where(x => x.Id == id));
    }
}
