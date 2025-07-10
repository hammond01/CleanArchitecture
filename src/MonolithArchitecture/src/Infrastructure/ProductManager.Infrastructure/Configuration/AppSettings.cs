using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
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

    // Configuration paths constants
    public static class ConfigPaths
    {
        public const string Database = $"{SectionName}:Database";
        public const string Security = $"{SectionName}:Security";
        public const string Logging = $"{SectionName}:Logging";
        public const string Cache = $"{SectionName}:Cache";

        public static class DatabasePaths
        {
            public const string DefaultConnection = $"{Database}:DefaultConnection";
            public const string IdentityConnection = $"{Database}:IdentityConnection";
        }

        public static class SecurityPaths
        {
            public const string JwtSecret = $"{Security}:JwtSecret";
            public const string JwtIssuer = $"{Security}:JwtIssuer";
            public const string JwtAudience = $"{Security}:JwtAudience";
            public const string BearerTokenExpiration = $"{Security}:BearerTokenExpiration";
            public const string RefreshTokenExpiration = $"{Security}:RefreshTokenExpiration";
            public const string EmailTokenLifetime = $"{Security}:EmailTokenLifetime";
            public const string PhoneNumberTokenLifetime = $"{Security}:PhoneNumberTokenLifetime";
            public const string ResetPasswordTokenLifetime = $"{Security}:ResetPasswordTokenLifetime";
            public const string TwoFactorTokenLifetime = $"{Security}:TwoFactorTokenLifetime";
            public const string OtpTokenLifetime = $"{Security}:OtpTokenLifetime";
            public const string RevokeUserSessionsDelay = $"{Security}:RevokeUserSessionsDelay";

            // Password policy paths
            public const string PasswordRequireDigit = $"{Security}:Password:RequireDigit";
            public const string PasswordRequiredLength = $"{Security}:Password:RequiredLength";
            public const string PasswordRequireNonAlphanumeric = $"{Security}:Password:RequireNonAlphanumeric";
            public const string PasswordRequireUppercase = $"{Security}:Password:RequireUppercase";
            public const string PasswordRequireLowercase = $"{Security}:Password:RequireLowercase";

            // Sign-in policy paths
            public const string SignInRequireConfirmedAccount = $"{Security}:SignIn:RequireConfirmedAccount";
        }
    }

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

    // Extended JWT token settings
    public TimeSpan BearerTokenExpiration { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromDays(7);
    public TimeSpan EmailTokenLifetime { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan PhoneNumberTokenLifetime { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan ResetPasswordTokenLifetime { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan TwoFactorTokenLifetime { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan OtpTokenLifetime { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan RevokeUserSessionsDelay { get; set; } = TimeSpan.FromSeconds(30);

    // Password policy settings
    public PasswordPolicySettings Password { get; set; } = new();

    // Sign-in settings
    public SignInPolicySettings SignIn { get; set; } = new();
}

public class PasswordPolicySettings
{
    public bool RequireDigit { get; set; } = false;
    public int RequiredLength { get; set; } = 6;
    public bool RequireNonAlphanumeric { get; set; } = false;
    public bool RequireUppercase { get; set; } = false;
    public bool RequireLowercase { get; set; } = false;
}

public class SignInPolicySettings
{
    public bool RequireConfirmedAccount { get; set; } = false;
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
        services.Configure<DatabaseSettings>(configuration.GetSection(AppSettings.ConfigPaths.Database));
        services.Configure<SecuritySettings>(configuration.GetSection(AppSettings.ConfigPaths.Security));
        services.Configure<LoggingSettings>(configuration.GetSection(AppSettings.ConfigPaths.Logging));
        services.Configure<CacheSettings>(configuration.GetSection(AppSettings.ConfigPaths.Cache));

        // Register SafeConfigurationAccessor for easy access to configuration
        services.AddSingleton<SafeConfigurationAccessor>();

        return services;
    }

    /// <summary>
    /// Gets a required configuration value and throws detailed exception if not found
    /// </summary>
    /// <param name="configuration">The configuration instance</param>
    /// <param name="key">The configuration key</param>
    /// <param name="callerMemberName">Automatically captured caller member name</param>
    /// <param name="callerFilePath">Automatically captured caller file path</param>
    /// <param name="callerLineNumber">Automatically captured caller line number</param>
    /// <returns>The configuration value</returns>
    /// <exception cref="InvalidOperationException">Thrown when configuration value is null or empty</exception>
    public static string GetRequiredValue(
        this IConfiguration configuration,
        string key,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            var fileName = Path.GetFileName(callerFilePath);
            throw new InvalidOperationException(
                $"❌ Configuration Error: Required setting '{key}' is missing or empty.\n" +
                $"📍 Called from: {callerMemberName} in {fileName}:{callerLineNumber}\n" +
                $"🔧 Fix: Add the following to your appsettings.json:\n" +
                $"   \"{key}\": \"your-value-here\"\n" +
                $"💡 Expected format: {GetExpectedFormat(key)}");
        }

        return value;
    }

    /// <summary>
    /// Gets a required configuration value of type T and throws detailed exception if not found
    /// </summary>
    public static T GetRequiredValue<T>(
        this IConfiguration configuration,
        string key,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var value = configuration.GetValue<T>(key);

        if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
        {
            var fileName = Path.GetFileName(callerFilePath);
            throw new InvalidOperationException(
                $"❌ Configuration Error: Required setting '{key}' is missing or invalid.\n" +
                $"📍 Called from: {callerMemberName} in {fileName}:{callerLineNumber}\n" +
                $"🔧 Fix: Add the following to your appsettings.json:\n" +
                $"   \"{key}\": {GetExpectedFormat(key)}\n" +
                $"💡 Expected type: {typeof(T).Name}");
        }

        return value;
    }

    /// <summary>
    /// Gets a required configuration section and throws detailed exception if not found
    /// </summary>
    public static IConfigurationSection GetRequiredSection(
        this IConfiguration configuration,
        string key,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var section = configuration.GetSection(key);

        if (!section.Exists() || !section.GetChildren().Any())
        {
            var fileName = System.IO.Path.GetFileName(callerFilePath);
            throw new InvalidOperationException(
                $"❌ Configuration Error: Required section '{key}' is missing or empty.\n" +
                $"📍 Called from: {callerMemberName} in {fileName}:{callerLineNumber}\n" +
                $"🔧 Fix: Add the following section to your appsettings.json:\n" +
                $"{GetSectionExample(key)}");
        }

        return section;
    }

    /// <summary>
    /// Validates that a configuration value meets minimum requirements
    /// </summary>
    public static string ValidateAndGet(
        this IConfiguration configuration,
        string key,
        int minLength = 0,
        string pattern = "",
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var value = configuration.GetRequiredValue(key, callerMemberName, callerFilePath, callerLineNumber);

        if (minLength > 0 && value.Length < minLength)
        {
            var fileName = System.IO.Path.GetFileName(callerFilePath);
            throw new InvalidOperationException(
                $"❌ Configuration Error: Setting '{key}' is too short (minimum {minLength} characters).\n" +
                $"📍 Called from: {callerMemberName} in {fileName}:{callerLineNumber}\n" +
                $"🔧 Current value length: {value.Length}, Required: {minLength}");
        }

        if (!string.IsNullOrEmpty(pattern) && !System.Text.RegularExpressions.Regex.IsMatch(value, pattern))
        {
            var fileName = System.IO.Path.GetFileName(callerFilePath);
            throw new InvalidOperationException(
                $"❌ Configuration Error: Setting '{key}' doesn't match required pattern.\n" +
                $"📍 Called from: {callerMemberName} in {fileName}:{callerLineNumber}\n" +
                $"🔧 Required pattern: {pattern}");
        }

        return value;
    }

    private static string GetExpectedFormat(string key)
    {
        return key.ToLower() switch
        {
            var k when k.Contains("secret") || k.Contains("password") => "\"your-secret-key-here\" (minimum 32 characters)",
            var k when k.Contains("connection") => "\"Data Source=server;Initial Catalog=database;...\"",
            var k when k.Contains("url") || k.Contains("issuer") || k.Contains("audience") => "\"https://your-domain.com\"",
            var k when k.Contains("email") => "\"admin@example.com\"",
            var k when k.Contains("timeout") => "30",
            var k when k.Contains("port") => "5000",
            var k when k.Contains("enabled") => "true",
            var k when k.Contains("minutes") => "60",
            var k when k.Contains("days") => "7",
            _ => "\"your-value-here\""
        };
    }

    private static string GetSectionExample(string key)
    {
        return key.ToLower() switch
        {
            "app:database" => @"""Database"": {
  ""DefaultConnection"": ""Data Source=server;Initial Catalog=database;..."",
  ""CommandTimeout"": 30,
  ""MaxRetryCount"": 3
}",
            "app:security" => @"""Security"": {
  ""JwtSecret"": ""your-secret-key-here"",
  ""JwtIssuer"": ""https://your-domain.com"",
  ""JwtAudience"": ""your-audience"",
  ""JwtExpirationMinutes"": 60
}",
            _ => $@"""{key}"": {{
  ""Property1"": ""value1"",
  ""Property2"": ""value2""
}}"
        };
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

        // Validate token lifetimes
        if (options.Security.BearerTokenExpiration <= TimeSpan.Zero)
            failures.Add("App.Security.BearerTokenExpiration must be greater than zero");

        if (options.Security.RefreshTokenExpiration <= TimeSpan.Zero)
            failures.Add("App.Security.RefreshTokenExpiration must be greater than zero");

        // Validate password policy
        if (options.Security.Password.RequiredLength < 1)
            failures.Add("App.Security.Password.RequiredLength must be at least 1");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
