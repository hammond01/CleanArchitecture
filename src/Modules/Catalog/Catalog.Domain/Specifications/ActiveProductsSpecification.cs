using BuildingBlocks.Domain.Specifications;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Specifications;

/// <summary>
/// Specification for getting active (non-discontinued) products
/// </summary>
public class ActiveProductsSpecification : BaseSpecification<Product>
{
    public ActiveProductsSpecification()
        : base(p => !p.Discontinued)
    {
        AddInclude(p => p.Category);
        AddOrderBy(p => p.ProductName);
    }

    public ActiveProductsSpecification(string categoryId)
        : base(p => !p.Discontinued && p.CategoryId == categoryId)
    {
        AddInclude(p => p.Category);
        AddOrderBy(p => p.ProductName);
    }
}
