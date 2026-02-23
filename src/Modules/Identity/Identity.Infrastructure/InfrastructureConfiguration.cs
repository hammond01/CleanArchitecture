using Identity.Domain.Repositories;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

/// <summary>
/// Infrastructure layer configuration for Identity module
/// </summary>
public static class InfrastructureConfiguration
{
    public static IServiceCollection AddIdentityInfrastructureServices(
        this IServiceCollection services,
        string connectionString)
    {
        // Register DbContext
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repository
        services.AddScoped<IIdentityRepository, IdentityRepository>();

        return services;
    }
}
