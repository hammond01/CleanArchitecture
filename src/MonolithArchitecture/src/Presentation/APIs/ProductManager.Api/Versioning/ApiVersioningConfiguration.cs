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
                    Description = "Product Manager API Version 1.0 - A comprehensive e-commerce product management system built with Clean Architecture principles.",
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
                }
            );

            options.SwaggerDoc(
                "v2",
                new OpenApiInfo
                {
                    Title = "Product Manager API",
                    Version = "v2",
                    Description = "Product Manager API Version 2.0 with enhanced features including advanced security, distributed locking, and comprehensive audit logging.",
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
                }
            );

            // Add JWT authentication to Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
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
                    Array.Empty<string>()
                }
            });            // Add operation filters for better documentation
            options.UseInlineDefinitionsForEnums();

            // Include XML comments if available
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Include XML comments from referenced projects
            var referencedXmlFiles = new[]
            {
                "ProductManager.Domain.xml",
                "ProductManager.Application.xml",
                "ProductManager.Shared.xml"
            };

            foreach (var xmlFileName in referencedXmlFiles)
            {
                var referencedXmlPath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
                if (File.Exists(referencedXmlPath))
                {
                    options.IncludeXmlComments(referencedXmlPath);
                }
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
