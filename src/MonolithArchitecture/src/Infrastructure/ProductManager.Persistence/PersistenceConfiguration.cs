using ProductManager.Domain.Entities.Identity;
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
        services.AddScoped<IAdminRepository, AdminRepository>();

        services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

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
