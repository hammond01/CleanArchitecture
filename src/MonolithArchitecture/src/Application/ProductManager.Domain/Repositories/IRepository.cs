namespace ProductManager.Domain.Repositories;

public interface IRepository
{
    Task Get(CancellationToken cancellationToken);
}  
