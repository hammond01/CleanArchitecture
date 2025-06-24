using Microsoft.Extensions.DependencyInjection;
using ProductManager.Domain.Identity;
using ProductManager.Infrastructure.DateTimes;
using ProductManager.Infrastructure.Identity;
using ProductManager.Infrastructure.Middleware;
using ProductManager.Infrastructure.Metrics;
using ProductManager.Infrastructure.Cache;
using ProductManager.Infrastructure.Configuration;
using ProductManager.Infrastructure.RateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
namespace ProductManager.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection InfrastructureConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUser, CurrentWebUser>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddDateTimeProvider();

        // Register action logging filter
        services.AddScoped<ActionLoggingFilter>();

        // Add application configuration validation
        services.AddAppConfiguration(configuration);        // Add application metrics
        services.AddApplicationMetrics();

        // Add cache services
        services.AddMemoryCache();
        services.AddDistributedMemoryCache(); // In-memory distributed cache for testing

        services.AddScoped<ICacheService, DistributedCacheService>();
        services.AddScoped<BusinessCacheService>();// Add rate limiting - simplified version
        services.AddSingleton(provider =>
        {
            var rateLimitConfig = configuration.GetSection("RateLimit");
            return new Infrastructure.RateLimit.RateLimitOptions
            {
                EnableRateLimit = rateLimitConfig.GetValue<bool>("EnableRateLimit", true),
                MaxRequests = rateLimitConfig.GetValue<int>("MaxRequests", 100),
                Window = TimeSpan.FromMinutes(rateLimitConfig.GetValue<int>("WindowMinutes", 1))
            };
        });

        return services;
    }
}
