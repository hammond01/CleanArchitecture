using BuildingBlocks.Domain.Specifications;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Specifications;

/// <summary>
/// Specification for getting products with category included
/// </summary>
public class ProductWithCategorySpecification : BaseSpecification<Product>
{
    public ProductWithCategorySpecification()
    {
        AddInclude(p => p.Category);
    }

    public ProductWithCategorySpecification(string productId)
        : base(p => p.Id == productId)
    {
        AddInclude(p => p.Category);
    }
}
