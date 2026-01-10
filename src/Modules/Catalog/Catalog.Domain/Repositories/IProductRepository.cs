using BuildingBlocks.Domain.Repositories;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Repositories;

/// <summary>
/// Repository interface for Product entity
/// </summary>
public interface IProductRepository : IRepository<Product, string>
{
    // Add custom methods specific to Product if needed
}
