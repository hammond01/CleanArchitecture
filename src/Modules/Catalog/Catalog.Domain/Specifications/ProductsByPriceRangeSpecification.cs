using BuildingBlocks.Domain.Specifications;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Specifications;

/// <summary>
/// Specification for getting products within a price range
/// </summary>
public class ProductsByPriceRangeSpecification : BaseSpecification<Product>
{
    public ProductsByPriceRangeSpecification(decimal minPrice, decimal maxPrice)
        : base(p => p.UnitPrice.HasValue && p.UnitPrice >= minPrice && p.UnitPrice <= maxPrice)
    {
        AddInclude(p => p.Category);
        AddOrderBy(p => p.UnitPrice ?? 0);
    }
}
