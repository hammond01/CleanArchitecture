using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProductManager.Api.Versioning; // Add this for API versioning
using ProductManager.Application;
using ProductManager.Constants.AuthorizationDefinitions;
using ProductManager.Infrastructure;
using ProductManager.Infrastructure.HealthChecks;
using ProductManager.Infrastructure.Middleware;
using ProductManager.Infrastructure.Storage;
using ProductManager.Persistence;
using Serilog;
using Serilog.Events;
using SolidTemplate.Constants.ConfigurationOptions;
var builder = WebApplication.CreateBuilder(args);

// Configure Serilog first
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
    "logs/application-.log",
    rollingInterval: RollingInterval.Day,
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
    fileSizeLimitBytes: 5 * 1024 * 1024,
    retainedFileCountLimit: 3,
    rollOnFileSizeLimit: true,
    shared: true,
    flushToDiskInterval: TimeSpan.FromSeconds(1)
    )
    .CreateLogger();

builder.Host.UseSerilog();

{
    builder.Services.AddControllers(options =>
    {
        // Add global action logging filter
        options.Filters.Add<ActionLoggingFilter>();
    })
    .AddJsonOptions(options =>
    {
        // Configure JSON serializer to handle object cycles
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
    builder.Services.AddEndpointsApiExplorer();

    // Add API Versioning Configuration
    builder.Services.AddApiVersioningConfiguration();
    builder.Services.AddSwaggerVersioning();    // Add Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck<DatabaseHealthCheck>("database")
        .AddCheck<ApplicationHealthCheck>("application"); builder.Services.AddPersistence(builder.Configuration.GetConnectionString("SQL")!);
    builder.Services.ApplicationConfigureServices();
    builder.Services.InfrastructureConfigureServices(builder.Configuration);
    builder.Services.Configure<IdentityConfig>(builder.Configuration.GetSection(IdentityConfig.ConfigName));
    var audience = builder.Configuration["IdentityConfig:AUDIENCE"];
    var issUser = builder.Configuration["IdentityConfig:ISSUER"];
    var key = builder.Configuration["IdentityConfig:SECRET"];

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidIssuer = issUser,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
        };
    });

    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = PasswordPolicy.RequireDigit;
        options.Password.RequiredLength = PasswordPolicy.RequiredLength;
        options.Password.RequireNonAlphanumeric = PasswordPolicy.RequireNonAlphanumeric;
        options.Password.RequireUppercase = PasswordPolicy.RequireUppercase;
        options.Password.RequireLowercase = PasswordPolicy.RequireLowercase;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 10;
        options.Lockout.AllowedForNewUsers = true;
    });

    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
        .AddEnvironmentVariables();
}

var app = builder.Build();
{
    // Add Serilog request logging with a detailed template
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (_, _, _) => LogEventLevel.Information;
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
        };
    });

    // Log startup
    Log.Information("🚀 ProductManager API starting up...");

    // Configure Health Check endpoints
    app.UseHealthChecks("/health");
    app.UseHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });
    app.UseHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => false
    });

    // Add detailed health check endpoint for development
    if (app.Environment.IsDevelopment())
    {
        app.UseHealthChecks("/health/detailed", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        exception = entry.Value.Exception?.Message,
                        duration = entry.Value.Duration.ToString(),
                        data = entry.Value.Data
                    })
                });
                await context.Response.WriteAsync(result);
            }
        });
    }

    if (app.Environment.IsDevelopment())
    {
        // Use versioned Swagger configuration
        app.UseSwaggerVersioning();
        Log.Information("📚 Swagger UI available at /swagger");
    }
    using (var serviceScope =
           ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        var databaseInitializer = serviceScope.ServiceProvider.GetService<IDatabaseInitializer>();
        databaseInitializer?.SeedAsync().Wait();
    }
    app.UseGlobalExceptionHandlerMiddleware();

    // Add API request logging middleware (logs all requests to a database)
    app.UseMiddleware<ApiRequestLoggingMiddleware>();

    // Add automatic entity locking middleware for PUT/DELETE operations
    app.UseMiddleware<AutoEntityLockMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();
    app.MapControllers();

    // Log that the server is ready
    Log.Information("✅ ProductManager API is ready!");
    Log.Information("🌐 Listening on: {Urls}", string.Join(", ", builder.WebHost.GetSetting("urls")?.Split(';') ?? new[]
    {
        "http://localhost:5000"
    }));

    // Ensure any buffered events are sent at shutdown
    app.Lifetime.ApplicationStopped.Register(() =>
    {
        Log.Information("🔴 ProductManager API shutting down...");
        Log.CloseAndFlush();
    }); app.Run();
}

// Make Program class accessible for integration tests
public partial class Program { }
