using BuildingBlocks.Domain.Specifications;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Specifications;

/// <summary>
/// Specification for getting products by category
/// </summary>
public class ProductsByCategorySpecification : BaseSpecification<Product>
{
    public ProductsByCategorySpecification(string categoryId)
        : base(p => p.CategoryId == categoryId)
    {
        AddInclude(p => p.Category);
        AddOrderBy(p => p.ProductName);
    }
}
