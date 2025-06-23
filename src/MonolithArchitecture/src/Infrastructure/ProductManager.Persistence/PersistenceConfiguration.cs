using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Domain.Entities.Identity;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;
using ProductManager.Infrastructure.Storage;
using ProductManager.Persistence.Extensions;
using ProductManager.Persistence.Repositories;
using ProductManager.Persistence.Services;
using ProductManager.Shared.Permission;
using ProductManager.Shared.Locks;
using ProductManager.Persistence.Locks;

namespace ProductManager.Persistence;

public static class PersistenceConfiguration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>(); services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

        // Register logging service
        services.AddScoped<IActionLogService, ActionLogService>();

        // Register lock manager
        services.AddScoped<ILockManager, LockManager>();

        services.AddSingleton<EntityPermissions>();
        services.AddScoped<IdentityExtension>();
        services.AddRepositories();
        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped(typeof(IUnitOfWork),
        implementationFactory: serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());
    }
}
