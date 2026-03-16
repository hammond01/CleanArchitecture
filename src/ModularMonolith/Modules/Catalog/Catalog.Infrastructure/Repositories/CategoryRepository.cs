using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Persistence;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category, string>, ICategoryRepository
{
    public CategoryRepository(CatalogDbContext context) : base(context)
    {
    }

    // Add custom methods specific to Category if needed
}
