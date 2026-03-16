using BuildingBlocks.Domain.Specifications;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Specifications;

/// <summary>
/// Specification for searching products by name
/// </summary>
public class ProductSearchSpecification : BaseSpecification<Product>
{
    public ProductSearchSpecification(string searchTerm)
        : base(p => p.ProductName.Contains(searchTerm ?? string.Empty))
    {
        AddInclude(p => p.Category);
        AddOrderBy(p => p.ProductName);
    }
}
