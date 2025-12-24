using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Product.Specifications;

/// <summary>
/// Specification to get products with their category included
/// Example of controlled eager loading
/// </summary>
public class ProductsWithCategorySpecification : BaseSpecification<Products>
{
    public ProductsWithCategorySpecification() : base()
    {
        // Include Category navigation property
        AddInclude(p => p.Category);

        // Disable tracking for better performance (read-only query)
        DisableTracking();
    }

    public ProductsWithCategorySpecification(string productId)
        : base(p => p.Id == productId)
    {
        // Include Category navigation property
        AddInclude(p => p.Category);

        // Disable tracking for better performance
        DisableTracking();
    }
}

/// <summary>
/// Specification to get products by category with pagination
/// Example of filtering + pagination + eager loading
/// </summary>
public class ProductsByCategorySpecification : BaseSpecification<Products>
{
    public ProductsByCategorySpecification(string categoryId, int pageNumber = 1, int pageSize = 10)
        : base(p => p.CategoryId == categoryId)
    {
        // Include Category
        AddInclude(p => p.Category);

        // Apply ordering
        ApplyOrderBy(p => p.ProductName);

        // Apply pagination
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);

        // Disable tracking
        DisableTracking();
    }
}

/// <summary>
/// Specification to get active (not discontinued) products
/// Example of business logic in specification
/// </summary>
public class ActiveProductsSpecification : BaseSpecification<Products>
{
    public ActiveProductsSpecification()
        : base(p => !p.Discontinued)
    {
        // Include Category
        AddInclude(p => p.Category);

        // Order by name
        ApplyOrderBy(p => p.ProductName);

        // Disable tracking
        DisableTracking();
    }
}

/// <summary>
/// Specification to get products with low stock
/// Example of complex filtering
/// </summary>
public class LowStockProductsSpecification : BaseSpecification<Products>
{
    public LowStockProductsSpecification(short threshold = 10)
        : base(p => p.UnitsInStock.HasValue && p.UnitsInStock.Value < threshold && !p.Discontinued)
    {
        // Include Category to show which category needs restocking
        AddInclude(p => p.Category);

        // Order by stock level (lowest first)
        ApplyOrderBy(p => p.UnitsInStock!);

        // Disable tracking
        DisableTracking();
    }
}

/// <summary>
/// Specification to search products by name
/// Example of text search with pagination
/// </summary>
public class ProductSearchSpecification : BaseSpecification<Products>
{
    public ProductSearchSpecification(string searchTerm, int pageNumber = 1, int pageSize = 20)
        : base(p => p.ProductName.Contains(searchTerm))
    {
        // Include Category
        AddInclude(p => p.Category);

        // Order by relevance (name)
        ApplyOrderBy(p => p.ProductName);

        // Apply pagination
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);

        // Disable tracking
        DisableTracking();
    }
}

/// <summary>
/// Specification to get products in price range
/// Example of range filtering
/// </summary>
public class ProductsByPriceRangeSpecification : BaseSpecification<Products>
{
    public ProductsByPriceRangeSpecification(decimal minPrice, decimal maxPrice)
        : base(p => p.UnitPrice.HasValue && p.UnitPrice.Value >= minPrice && p.UnitPrice.Value <= maxPrice)
    {
        // Include Category
        AddInclude(p => p.Category);

        // Order by price
        ApplyOrderBy(p => p.UnitPrice!);

        // Disable tracking
        DisableTracking();
    }
}

/// <summary>
/// Specification for updating product (needs tracking)
/// Example of tracked query for updates
/// </summary>
public class ProductForUpdateSpecification : BaseSpecification<Products>
{
    public ProductForUpdateSpecification(string productId)
        : base(p => p.Id == productId)
    {
        // Include Category for validation
        AddInclude(p => p.Category);

        // Enable tracking for update scenario
        EnableTracking();
    }
}
