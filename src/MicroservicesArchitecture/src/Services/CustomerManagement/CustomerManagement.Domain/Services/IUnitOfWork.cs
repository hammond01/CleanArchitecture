namespace CustomerManagement.Domain.Services;

public interface IUnitOfWork
{
  Task<int> SaveChangesAsync();
  Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
