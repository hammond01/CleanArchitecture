using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Repositories;

/// <summary>
/// Base repository interface
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Product repository interface
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetBySupplierIdAsync(string supplierId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetDiscontinuedProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetOutOfStockProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    Task<Product?> GetByNameAsync(string productName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}

/// <summary>
/// Category repository interface
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByNameAsync(string categoryName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<int> GetProductCountAsync(string categoryId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Supplier repository interface
/// </summary>
public interface ISupplierRepository : IRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetActiveSuppliersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Supplier>> GetSuppliersWithProductsAsync(CancellationToken cancellationToken = default);
    Task<Supplier?> GetByCompanyNameAsync(string companyName, CancellationToken cancellationToken = default);
    Task<IEnumerable<Supplier>> GetByRatingAsync(SupplierRating rating, CancellationToken cancellationToken = default);
    Task<IEnumerable<Supplier>> GetByCountryAsync(string country, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(string supplierId, CancellationToken cancellationToken = default);
    Task<int> GetProductCountAsync(string supplierId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Unit of work interface
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    ISupplierRepository Suppliers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
