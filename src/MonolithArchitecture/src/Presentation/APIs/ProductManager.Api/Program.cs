using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProductManager.Application;
using ProductManager.Constants.AuthorizationDefinitions;
using ProductManager.Infrastructure;
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
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddPersistence(builder.Configuration.GetConnectionString("SQL")!);
    builder.Services.ApplicationConfigureServices();
    builder.Services.InfrastructureConfigureServices();
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

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
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
    });

    app.Run();
}
