using Identity.Application;
using Identity.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Api.Extensions;

/// <summary>
/// Identity module extension methods for dependency injection
/// </summary>
public static class IdentityModuleExtensions
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string? connectionString = null)
    {
        // Add application layer services
        services.AddIdentityApplicationServices();

        // Add infrastructure services
        var connString = connectionString ?? "Server=.;Database=CleanArchitectureIdentity;Integrated Security=true;TrustServerCertificate=true;";
        services.AddIdentityInfrastructureServices(configuration, environment, connString);

        return services;
    }
}
