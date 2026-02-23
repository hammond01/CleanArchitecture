using BuildingBlocks.Domain.Specifications;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Specifications;

/// <summary>
/// Specification for getting category with all its products
/// </summary>
public class CategoryWithProductsSpecification : BaseSpecification<Category>
{
    public CategoryWithProductsSpecification()
    {
        AddInclude("Products");
    }

    public CategoryWithProductsSpecification(string categoryId)
        : base(c => c.Id == categoryId)
    {
        AddInclude("Products");
    }
}
