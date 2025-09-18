using CustomerManagement.Domain.Services;
using CustomerManagement.Infrastructure.Data;

namespace CustomerManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
  private readonly CustomerManagementDbContext _context;

  public UnitOfWork(CustomerManagementDbContext context)
  {
    _context = context;
  }

  public async Task<int> SaveChangesAsync()
  {
    return await _context.SaveChangesAsync();
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
  {
    return await _context.SaveChangesAsync(cancellationToken);
  }
}
