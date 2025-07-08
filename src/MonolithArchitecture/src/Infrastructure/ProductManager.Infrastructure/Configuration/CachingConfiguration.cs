using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ProductManager.Infrastructure.Configuration;

public static class CachingConfiguration
{
    public static IServiceCollection AddCachingConfiguration(this IServiceCollection services)
    {
        // Add Response Caching
        services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024 * 1024; // 1MB
            options.UseCaseSensitivePaths = false;
        });

        // Add Output Caching (.NET 7+)
        services.AddOutputCache(options =>
        {
            // Default policy
            options.AddBasePolicy(builder =>
                builder.Expire(TimeSpan.FromMinutes(5)));

            // Products cache policy
            options.AddPolicy("Products", builder =>
                builder
                    .Expire(TimeSpan.FromMinutes(10))
                    .SetVaryByQuery("page", "pageSize", "search")
                    .Tag("products"));

            // Categories cache policy
            options.AddPolicy("Categories", builder =>
                builder
                    .Expire(TimeSpan.FromMinutes(30))
                    .Tag("categories"));

            // User-specific cache
            options.AddPolicy("UserSpecific", builder =>
                builder
                    .Expire(TimeSpan.FromMinutes(5))
                    .SetVaryByHeader("Authorization")
                    .Tag("user-data"));
        });

        return services;
    }

    public static IApplicationBuilder UseCachingConfiguration(this IApplicationBuilder app)
    {
        app.UseResponseCaching();
        app.UseOutputCache();

        // Add cache headers middleware
        app.Use(async (context, next) =>
        {
            // Add cache control headers for static content
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/health"))
            {
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";
            }

            await next();
        });

        return app;
    }
}
