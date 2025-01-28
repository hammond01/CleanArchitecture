namespace ProductManager.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection InfrastructureConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, CurrentWebUser>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddDateTimeProvider();
        return services;
    }
}
