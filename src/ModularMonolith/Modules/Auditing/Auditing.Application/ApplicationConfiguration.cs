using FluentValidation;
using BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Auditing.Application;

/// <summary>
/// Application layer configuration for Auditing module
/// </summary>
public static class ApplicationConfiguration
{
    public static IServiceCollection AddAuditingApplicationServices(this IServiceCollection services)
    {
        // Register Auditing module handlers
        services.AddHandlersFromAssembly(typeof(ApplicationConfiguration).Assembly);

        // Register validators
        services.AddValidatorsFromAssembly(typeof(ApplicationConfiguration).Assembly);

        return services;
    }
}
