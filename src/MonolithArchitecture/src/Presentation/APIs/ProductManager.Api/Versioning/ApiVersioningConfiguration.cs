// API Versioning Configuration for .NET 8
using Asp.Versioning;
using Microsoft.OpenApi.Models;

namespace ProductManager.Api.Versioning;

public static class ApiVersioningConfiguration
{
    public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                // Default version when no version is specified
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;

                // Support multiple versioning strategies
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("version"),
                    new QueryStringApiVersionReader("v"),
                    new HeaderApiVersionReader("X-API-Version"),
                    new UrlSegmentApiVersionReader()
                );
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    public static IServiceCollection AddSwaggerVersioning(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Product Manager API",
                    Version = "v1",
                    Description = "Product Manager API Version 1.0",
                }
            );

            options.SwaggerDoc(
                "v2",
                new OpenApiInfo
                {
                    Title = "Product Manager API",
                    Version = "v2",
                    Description = "Product Manager API Version 2.0 with enhanced features",
                }
            );

            // Include XML comments if available
            var xmlFile =
                $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });
    }

    public static WebApplication UseSwaggerVersioning(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Manager API V1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "Product Manager API V2");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "Product Manager API Documentation";
            });
        }

        return app;
    }
}
