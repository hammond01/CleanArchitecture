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
    public static IServiceCollection AddIdentityPersistence(this IServiceCollection services, string connectionString,
        string migrationsAssembly = "")
    {
        services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlServer(connectionString,
            sqlServerOptionsAction: sql =>
            {
                if (!string.IsNullOrEmpty(migrationsAssembly))
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                }
            }))
            .AddDbContextFactory<ApplicationIdentityDbContext>((Action<DbContextOptionsBuilder>)null!, ServiceLifetime.Scoped);

        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IdentityExtension>();
        services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
        services.AddSingleton<EntityPermissions>();
        return services;
    }
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(ICategoryRepository), typeof(CategoryRepository));
        services.AddScoped(typeof(IUnitOfWork),
        implementationFactory: serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());
        return services;
    }
}
