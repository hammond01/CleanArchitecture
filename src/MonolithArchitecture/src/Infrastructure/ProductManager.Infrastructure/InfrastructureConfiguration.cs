using ProductManager.Infrastructure.Identity;
namespace ProductManager.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection InfrastructureConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, CurrentWebUser>();
        return services;
    }
}
