using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ProductManager.Infrastructure.Configuration;

/// <summary>
/// Application configuration options
/// </summary>
public class AppSettings
{
    public const string SectionName = "App";

    [Required]
    public string Name { get; set; } = "ProductManager";

    [Required]
    public string Version { get; set; } = "1.0.0";

    public string Environment { get; set; } = "Development";

    public DatabaseSettings Database { get; set; } = new();
    public SecuritySettings Security { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();
    public CacheSettings Cache { get; set; } = new();
}

public class DatabaseSettings
{
    [Required]
    public string DefaultConnection { get; set; } = string.Empty;

    [Required]
    public string IdentityConnection { get; set; } = string.Empty;

    public int CommandTimeout { get; set; } = 30;
    public int MaxRetryCount { get; set; } = 3;
    public bool EnableSensitiveDataLogging { get; set; } = false;
}

public class SecuritySettings
{
    [Required]
    public string JwtSecret { get; set; } = string.Empty;

    [Required]
    public string JwtIssuer { get; set; } = string.Empty;

    [Required]
    public string JwtAudience { get; set; } = string.Empty;

    public int JwtExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
    public bool RequireHttps { get; set; } = true;
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}

public class LoggingSettings
{
    public string LogLevel { get; set; } = "Information";
    public bool EnableFileLogging { get; set; } = true;
    public string LogPath { get; set; } = "logs";
    public int MaxFileSizeMB { get; set; } = 10;
    public int MaxFiles { get; set; } = 5;
    public bool EnableStructuredLogging { get; set; } = true;
}

public class CacheSettings
{
    public bool EnableDistributedCache { get; set; } = false;
    public string ConnectionString { get; set; } = string.Empty;
    public int DefaultExpirationMinutes { get; set; } = 30;
}

/// <summary>
/// Configuration validation and registration
/// </summary>
public static class ConfigurationExtensions
{
    public static IServiceCollection AddAppConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register and validate configuration sections
        services.Configure<AppSettings>(configuration.GetSection(AppSettings.SectionName));
        services.AddSingleton<IValidateOptions<AppSettings>, AppSettingsValidator>();

        // Register individual sections for easier injection
        services.Configure<DatabaseSettings>(configuration.GetSection($"{AppSettings.SectionName}:Database"));
        services.Configure<SecuritySettings>(configuration.GetSection($"{AppSettings.SectionName}:Security"));
        services.Configure<LoggingSettings>(configuration.GetSection($"{AppSettings.SectionName}:Logging"));
        services.Configure<CacheSettings>(configuration.GetSection($"{AppSettings.SectionName}:Cache"));

        return services;
    }
}

/// <summary>
/// Configuration validator
/// </summary>
public class AppSettingsValidator : IValidateOptions<AppSettings>
{
    public ValidateOptionsResult Validate(string? name, AppSettings options)
    {
        var failures = new List<string>();

        // Validate required settings
        if (string.IsNullOrWhiteSpace(options.Name))
            failures.Add("App.Name is required");

        if (string.IsNullOrWhiteSpace(options.Version))
            failures.Add("App.Version is required");

        if (string.IsNullOrWhiteSpace(options.Database.DefaultConnection))
            failures.Add("App.Database.DefaultConnection is required");

        if (string.IsNullOrWhiteSpace(options.Database.IdentityConnection))
            failures.Add("App.Database.IdentityConnection is required");

        if (string.IsNullOrWhiteSpace(options.Security.JwtSecret))
            failures.Add("App.Security.JwtSecret is required");

        if (options.Security.JwtSecret.Length < 32)
            failures.Add("App.Security.JwtSecret must be at least 32 characters long");

        if (string.IsNullOrWhiteSpace(options.Security.JwtIssuer))
            failures.Add("App.Security.JwtIssuer is required");

        if (string.IsNullOrWhiteSpace(options.Security.JwtAudience))
            failures.Add("App.Security.JwtAudience is required");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
