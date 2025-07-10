using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities.Identity;
using ProductManager.Domain.Repositories;
using ProductManager.Infrastructure.Configuration;
using ProductManager.Infrastructure.Storage;
using ProductManager.Persistence.Extensions;
using ProductManager.Persistence.Locks;
using ProductManager.Persistence.Repositories;
using ProductManager.Persistence.Services;
using ProductManager.Shared.Locks;
using ProductManager.Shared.Permission;

namespace ProductManager.Persistence;

public static class PersistenceConfiguration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // ONLY use AppSettings with safe configuration access
        var connectionString = configuration.GetRequiredValue(AppSettings.ConfigPaths.DatabasePaths.DefaultConnection);
        var commandTimeout = configuration.GetRequiredValue<int>($"{AppSettings.ConfigPaths.Database}:CommandTimeout");
        var maxRetryCount = configuration.GetRequiredValue<int>($"{AppSettings.ConfigPaths.Database}:MaxRetryCount");
        var enableSensitiveDataLogging = configuration.GetRequiredValue<bool>($"{AppSettings.ConfigPaths.Database}:EnableSensitiveDataLogging");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(commandTimeout);
                sqlOptions.EnableRetryOnFailure(maxRetryCount);
            })
            .EnableSensitiveDataLogging(enableSensitiveDataLogging));

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

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
