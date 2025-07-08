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
            // Version 1.0
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Product Manager API",
                    Version = "v1",
                    Description = """
                        Product Manager API Version 1.0 - A comprehensive e-commerce product management system built with Clean Architecture principles.

                        ## Features:
                        - 🔐 JWT Authentication with refresh tokens
                        - 🚦 Rate limiting and throttling
                        - 📊 Health checks and monitoring
                        - 🔄 Response caching
                        - 🛡️ Advanced security headers
                        - 📝 Comprehensive logging
                        - 🔒 Entity locking for concurrent operations

                        ## Getting Started:
                        1. Register an account via `/api/v1/identity/register`
                        2. Login to get access token via `/api/v1/identity/login`
                        3. Include the Bearer token in Authorization header
                        4. Start managing your products!
                        """,
                    Contact = new OpenApiContact
                    {
                        Name = "Hammond",
                        Email = "Hieutruonghoang01@gmail.com",
                        Url = new Uri("https://github.com/hammond01")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

            // Version 2.0
            options.SwaggerDoc(
                "v2",
                new OpenApiInfo
                {
                    Title = "Product Manager API",
                    Version = "v2",
                    Description = "Product Manager API Version 2.0 with enhanced features and improved performance.",
                    Contact = new OpenApiContact
                    {
                        Name = "Hammond",
                        Email = "Hieutruonghoang01@gmail.com",
                    }
                });

            // Add JWT Authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by a space and your JWT token.\n\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });

            // Add API versioning
            options.AddSecurityDefinition("ApiVersion", new OpenApiSecurityScheme
            {
                Name = "X-API-Version",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = "API Version Header (optional, defaults to v1.0)"
            });

            // Include XML comments if available
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Add examples (commented out for now)
            // options.SchemaFilter<ExampleSchemaFilter>();
            // options.OperationFilter<ExampleOperationFilter>();

            // Group by controller for better organization
            options.TagActionsBy(api =>
            {
                var controllerName = api.ActionDescriptor.RouteValues["controller"];
                return [controllerName ?? "Unknown"];
            });

            options.DocInclusionPredicate((name, api) => true);

            // Custom operation ordering by controller then method
            options.OrderActionsBy((apiDesc) =>
            {
                var controller = apiDesc.ActionDescriptor.RouteValues["controller"] ?? "Unknown";
                var action = apiDesc.ActionDescriptor.RouteValues["action"] ?? "Unknown";
                return $"{controller}_{action}";
            });
        });
    }

    public static IApplicationBuilder UseSwaggerVersioning(this IApplicationBuilder app)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Manager API v1.0");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "Product Manager API v2.0");

            options.RoutePrefix = "swagger";
            options.DocumentTitle = "Product Manager API Documentation";
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.EnableFilter();
            options.ShowExtensions();
            options.EnableValidator();

            // Custom CSS for better appearance
            options.InjectStylesheet("/swagger-ui/custom.css");

            // OAuth2 configuration if needed
            options.OAuthClientId("swagger-ui");
            options.OAuthClientSecret("swagger-ui-secret");
            options.OAuthRealm("swagger-ui-realm");
        });

        return app;
    }
}

// TODO: Example schema filter for better documentation (commented out for now)
/*
public class ExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Add examples for DTOs
    }
}

// TODO: Example operation filter (commented out for now)
public class ExampleOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add response examples
    }
}
*/
