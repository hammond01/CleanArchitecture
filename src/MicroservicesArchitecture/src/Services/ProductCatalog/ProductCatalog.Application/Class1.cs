namespace ProductCatalog.Application.Common.Interfaces;

/// <summary>
/// Product repository interface
/// </summary>
public interface IProductRepository
{
  Task<Domain.Entities.Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<IEnumerable<Domain.Entities.Product>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<Domain.Entities.Product>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
  Task<IEnumerable<Domain.Entities.Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
  Task<Domain.Entities.Product> AddAsync(Domain.Entities.Product product, CancellationToken cancellationToken = default);
  Task UpdateAsync(Domain.Entities.Product product, CancellationToken cancellationToken = default);
  Task DeleteAsync(int id, CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Category repository interface
/// </summary>
public interface ICategoryRepository
{
  Task<Domain.Entities.Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<IEnumerable<Domain.Entities.Category>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<Domain.Entities.Category> AddAsync(Domain.Entities.Category category, CancellationToken cancellationToken = default);
  Task UpdateAsync(Domain.Entities.Category category, CancellationToken cancellationToken = default);
  Task DeleteAsync(int id, CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Supplier repository interface
/// </summary>
public interface ISupplierRepository
{
  Task<Domain.Entities.Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
  Task<IEnumerable<Domain.Entities.Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<Domain.Entities.Supplier> AddAsync(Domain.Entities.Supplier supplier, CancellationToken cancellationToken = default);
  Task UpdateAsync(Domain.Entities.Supplier supplier, CancellationToken cancellationToken = default);
  Task DeleteAsync(int id, CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Unit of work interface
/// </summary>
public interface IUnitOfWork
{
  IProductRepository Products { get; }
  ICategoryRepository Categories { get; }
  ISupplierRepository Suppliers { get; }
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  Task BeginTransactionAsync(CancellationToken cancellationToken = default);
  Task CommitTransactionAsync(CancellationToken cancellationToken = default);
  Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
