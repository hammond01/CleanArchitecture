using ProductManager.Domain.Repositories;
namespace ProductManager.Persistence.Repositories;

public class Repository : IRepository
{
    public Task Get(CancellationToken cancellationToken) => throw new NotImplementedException();
}
