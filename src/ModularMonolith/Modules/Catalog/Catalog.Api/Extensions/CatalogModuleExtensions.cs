using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Repositories;
using FluentValidation;
using BuildingBlocks.Application;
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
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(connectionString,
                builder => builder.MigrationsHistoryTable("__ef_migrations_history", "catalog"))
                .UseSnakeCaseNamingConvention());

        // Register UnitOfWork
        services.AddScoped<BuildingBlocks.Domain.Repositories.IUnitOfWork>(provider =>
            provider.GetRequiredService<CatalogDbContext>());

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Register Catalog module handlers from Catalog.Application
        services.AddHandlersFromAssembly(typeof(Application.Features.Products.Queries.GetProductsQuery).Assembly);

        // Register FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<Application.Validators.CreateOrUpdateProductCommandValidator>();

        return services;
    }
}

