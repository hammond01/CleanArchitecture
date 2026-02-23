using Auditing.Application;
using Auditing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auditing.Api.Extensions;

/// <summary>
/// Auditing module extension methods for dependency injection
/// </summary>
public static class AuditingModuleExtensions
{
    public static IServiceCollection AddAuditingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Add application layer services
        services.AddAuditingApplicationServices();

        // Add infrastructure services
        services.AddAuditingInfrastructureServices(connectionString);

        return services;
    }
}
