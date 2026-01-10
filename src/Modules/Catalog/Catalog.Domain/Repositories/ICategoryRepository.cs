using BuildingBlocks.Domain.Repositories;
using Catalog.Domain.Entities;

namespace Catalog.Domain.Repositories;

/// <summary>
/// Repository interface for Category entity
/// </summary>
public interface ICategoryRepository : IRepository<Category, string>
{
    // Add custom methods specific to Category if needed
}
