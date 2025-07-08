using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ProductManager.Infrastructure.Configuration;

public static class CorsConfiguration
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        return services.AddCors(options =>
        {
            options.AddPolicy("AllowedOrigins", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:3000",     // React dev server
                        "http://localhost:5173",     // Vite dev server
                        "https://localhost:7000",    // HTTPS local
                        "https://yourdomain.com"     // Production domain
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(30));
            });

            // Restrictive policy for production
            options.AddPolicy("Production", policy =>
            {
                policy
                    .WithOrigins("https://yourdomain.com")
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Authorization", "Content-Type", "Accept", "X-API-Version")
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromHours(1));
            });

            // Development policy - more permissive
            options.AddPolicy("Development", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }
}
