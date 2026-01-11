using Catalog.Application.Features.Categories.Commands;
using Catalog.Application.Features.Categories.Queries;
using Catalog.Application.Features.Products.Commands;
using Catalog.Application.Features.Products.Queries;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Repositories;
using FluentValidation;
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

        // Register UnitOfWork
        services.AddScoped<BuildingBlocks.Domain.Repositories.IUnitOfWork>(provider =>
            provider.GetRequiredService<CatalogDbContext>());

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Register Product handlers
        services.AddScoped<GetProductsQueryHandler>();
        services.AddScoped<GetProductByIdQueryHandler>();
        services.AddScoped<CreateOrUpdateProductCommandHandler>();
        services.AddScoped<DeleteProductCommandHandler>();

        // Register Category handlers
        services.AddScoped<GetCategoriesQueryHandler>();
        services.AddScoped<GetCategoryByIdQueryHandler>();
        services.AddScoped<CreateOrUpdateCategoryCommandHandler>();
        services.AddScoped<DeleteCategoryCommandHandler>();

        // Register FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<Application.Validators.CreateOrUpdateProductCommandValidator>();

        return services;
    }
}
