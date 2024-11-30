using ProductManager.Domain.Entities.Identity;
namespace ProductManager.Persistence;

public static class PersistenceConfiguration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();

        services.AddScoped<IdentityExtension>();
        services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
        services.AddSingleton<EntityPermissions>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped(typeof(IUnitOfWork),
        implementationFactory: serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
