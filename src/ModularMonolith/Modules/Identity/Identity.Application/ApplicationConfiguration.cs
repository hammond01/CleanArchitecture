using FluentValidation;
using BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application;

/// <summary>
/// Application layer configuration for Identity module
/// </summary>
public static class ApplicationConfiguration
{
    public static IServiceCollection AddIdentityApplicationServices(this IServiceCollection services)
    {
        // Register custom Dispatcher and handlers
        services.AddApplicationServices();
        services.AddHandlersFromAssembly(typeof(ApplicationConfiguration).Assembly);

        // Register validators
        services.AddValidatorsFromAssembly(typeof(ApplicationConfiguration).Assembly);

        return services;
    }
}
