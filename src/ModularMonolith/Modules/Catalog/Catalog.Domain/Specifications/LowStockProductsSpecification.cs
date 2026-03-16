using BuildingBlocks.Domain.Specifications;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Specifications;

/// <summary>
/// Specification for getting products with low stock
/// </summary>
public class LowStockProductsSpecification : BaseSpecification<Product>
{
    public LowStockProductsSpecification()
        : base(p => p.UnitsInStock.HasValue && p.ReorderLevel.HasValue && p.UnitsInStock <= p.ReorderLevel)
    {
        AddInclude(p => p.Category);
        AddOrderBy(p => p.UnitsInStock ?? 0);
    }
}
