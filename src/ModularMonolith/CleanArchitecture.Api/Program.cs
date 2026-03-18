using System.Net;
using System.Threading.RateLimiting;
using Serilog;
using Catalog.Api.Extensions;
using Identity.Api.Extensions;
using Auditing.Api.Extensions;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Auditing;
using BuildingBlocks.Application.Security;
using Catalog.Infrastructure.Persistence;
using CleanArchitecture.Api.Auditing;
using CleanArchitecture.Api.Configuration;
using CleanArchitecture.Api.HealthChecks;
using CleanArchitecture.Api.Middleware;
using CleanArchitecture.Api.Security;
using Identity.Infrastructure.Persistence;
using Auditing.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var apiSecurityOptions = builder.Configuration.GetSection(ApiSecurityOptions.SectionName).Get<ApiSecurityOptions>() ?? new ApiSecurityOptions();
var authRateLimitOptions = builder.Configuration.GetSection(AuthRateLimitOptions.SectionName).Get<AuthRateLimitOptions>() ?? new AuthRateLimitOptions();

if (!builder.Environment.IsDevelopment()
    && !builder.Environment.IsEnvironment("Test")
    && apiSecurityOptions.AllowedOrigins.Length == 0)
{
    throw new InvalidOperationException("ApiSecurity:AllowedOrigins must contain at least one origin outside Development/Test.");
}

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
builder.Services.AddScoped<IEntityChangeBuffer, EntityChangeBuffer>();
builder.Services.AddScoped<IAuditOutboxProcessor, AuditOutboxProcessor>();
builder.Services.AddHostedService<AuditOutboxWorker>();
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var firstError = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .FirstOrDefault() ?? "Validation failed.";

        var response = BuildingBlocks.Api.Responses.ApiResponse.CreateError(firstError, StatusCodes.Status400BadRequest);
        return new BadRequestObjectResult(response);
    };
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CleanArchitecture API",
        Version = "v1",
        Description = "Modular monolith API starter kit with Identity, Catalog, and Auditing modules."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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
    });
});
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<DbContextHealthCheck<CatalogDbContext>>("catalog-db", tags: ["ready"])
    .AddCheck<DbContextHealthCheck<IdentityDbContext>>("identity-db", tags: ["ready"])
    .AddCheck<DbContextHealthCheck<AuditingDbContext>>("auditing-db", tags: ["ready"]);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    foreach (var proxy in apiSecurityOptions.KnownProxies)
    {
        if (IPAddress.TryParse(proxy, out var parsedProxy))
        {
            options.KnownProxies.Add(parsedProxy);
        }
    }

    if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Test"))
    {
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    }
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/problem+json";
        var problem = new ProblemDetails
        {
            Title = "Too many requests.",
            Status = StatusCodes.Status429TooManyRequests,
            Detail = "The request rate limit for this endpoint has been exceeded.",
            Instance = context.HttpContext.Request.Path
        };

        await context.HttpContext.Response.WriteAsJsonAsync(problem, cancellationToken: token);
    };

    options.AddPolicy("AuthEndpoints", httpContext =>
    {
        var identity = httpContext.User.Identity;
        var partitionKey = identity?.IsAuthenticated == true
            ? $"{httpContext.Request.Path}:{httpContext.User.Identity!.Name}"
            : $"{httpContext.Request.Path}:{httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = authRateLimitOptions.PermitLimit,
                Window = TimeSpan.FromSeconds(authRateLimitOptions.WindowSeconds),
                QueueLimit = authRateLimitOptions.QueueLimit,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = true
            });
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("ConfiguredOrigins", policy =>
    {
        if (apiSecurityOptions.AllowedOrigins.Length == 0)
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
            return;
        }

        policy.WithOrigins(apiSecurityOptions.AllowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();

        if (apiSecurityOptions.AllowCredentials)
        {
            policy.AllowCredentials();
        }
    });
});

// Register shared application services once at composition root.
builder.Services.AddApplicationServices();

// Register modules
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=.;Database=CleanArchitecture;Integrated Security=true;TrustServerCertificate=true;";

builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration, builder.Environment, connectionString);
builder.Services.AddAuditingModule(builder.Configuration);

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseForwardedHeaders();
app.UseRequestAuditMiddleware();
app.UseExceptionHandlingMiddleware();
app.UseSecurityHeadersMiddleware();
app.UseHttpsRedirection();
app.UseCors("ConfiguredOrigins");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = HealthCheckResponseWriter.WriteJsonAsync
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = HealthCheckResponseWriter.WriteJsonAsync
});
app.MapControllers();

try
{
    Log.Information("Starting Clean Architecture API Gateway");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

// Make Program class accessible for integration tests
public partial class Program { }
