using Catalog.Application.Features.Products.Commands;
using Catalog.Application.Features.Products.Queries;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Api.Extensions;

public static class CatalogModuleExtensions
{
    public static IServiceCollection AddCatalogModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        var connectionString = configuration.GetConnectionString("Catalog")
            ?? configuration.GetConnectionString("Default");
            
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Register handlers
        services.AddScoped<GetProductsQueryHandler>();
        services.AddScoped<GetProductByIdQueryHandler>();
        services.AddScoped<CreateOrUpdateProductCommandHandler>();
        services.AddScoped<DeleteProductCommandHandler>();

        return services;
    }
}
