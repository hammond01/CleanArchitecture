using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Mappings;
using OrderManagement.Domain.Services;

namespace OrderManagement.Application;

/// <summary>
/// Application layer service extensions
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Add application services to the service collection
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Add AutoMapper
        services.AddAutoMapper(typeof(OrderMappingProfile));

        // Add FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Add Domain Services
        services.AddScoped<IOrderDomainService, OrderDomainService>();

        return services;
    }
}
