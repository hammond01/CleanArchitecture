using Microsoft.Extensions.DependencyInjection;
using ProductManager.Domain.Identity;
using ProductManager.Infrastructure.DateTimes;
using ProductManager.Infrastructure.Identity;
using ProductManager.Infrastructure.Middleware;
namespace ProductManager.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection InfrastructureConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, CurrentWebUser>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddDateTimeProvider();

        // Register action logging filter
        services.AddScoped<ActionLoggingFilter>();

        return services;
    }
}
