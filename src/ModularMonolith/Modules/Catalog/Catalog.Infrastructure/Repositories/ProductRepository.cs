using BuildingBlocks.Infrastructure.Persistence;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Persistence;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : Repository<Product, string>, IProductRepository
{
    public ProductRepository(CatalogDbContext context) : base(context)
    {
    }

    // Add custom methods specific to Product if needed
}
