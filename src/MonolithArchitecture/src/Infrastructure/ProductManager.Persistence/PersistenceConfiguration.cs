using Microsoft.Extensions.DependencyInjection;
using ProductManager.Persistence.Repositories;
namespace ProductManager.Persistence;

public static class PersistenceConfiguration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString,
        string migrationsAssembly = "")
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString, sqlServerOptionsAction: sql =>
            {
                if (!string.IsNullOrEmpty(migrationsAssembly))
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                }
            }))
            .AddDbContextFactory<ApplicationDbContext>((Action<DbContextOptionsBuilder>)null!, ServiceLifetime.Scoped)
            .AddRepositories();
        return services;
    }
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(ICategoryRepository), typeof(CategoryRepository));

        services.AddScoped(typeof(IUnitOfWork),
        implementationFactory: serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());
        return services;
    }
}
