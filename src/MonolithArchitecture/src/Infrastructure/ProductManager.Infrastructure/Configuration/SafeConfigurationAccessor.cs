using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ProductManager.Infrastructure.Configuration;

/// <summary>
/// Safe configuration accessor that provides type-safe access to AppSettings with detailed error messages
/// </summary>
public class SafeConfigurationAccessor
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public SafeConfigurationAccessor(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets AppSettings with validation
    /// </summary>
    public AppSettings GetAppSettings()
    {
        try
        {
            var appSettings = _serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
            return appSettings;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"❌ Configuration Error: Failed to load AppSettings.\n" +
                $"🔧 Fix: Ensure your appsettings.json contains a valid 'App' section.\n" +
                $"💡 Original error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets Database settings with validation
    /// </summary>
    public DatabaseSettings GetDatabaseSettings()
    {
        try
        {
            var databaseSettings = _serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;

            // Additional validation
            if (string.IsNullOrWhiteSpace(databaseSettings.DefaultConnection))
                throw new InvalidOperationException("DefaultConnection is required but not configured.");

            if (string.IsNullOrWhiteSpace(databaseSettings.IdentityConnection))
                throw new InvalidOperationException("IdentityConnection is required but not configured.");

            return databaseSettings;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"❌ Configuration Error: Failed to load Database settings.\n" +
                $"🔧 Fix: Ensure your appsettings.json contains a valid 'App:Database' section with DefaultConnection and IdentityConnection.\n" +
                $"💡 Original error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets Security settings with validation
    /// </summary>
    public SecuritySettings GetSecuritySettings()
    {
        try
        {
            var securitySettings = _serviceProvider.GetRequiredService<IOptions<SecuritySettings>>().Value;

            // Additional validation
            if (string.IsNullOrWhiteSpace(securitySettings.JwtSecret))
                throw new InvalidOperationException("JwtSecret is required but not configured.");

            if (securitySettings.JwtSecret.Length < 32)
                throw new InvalidOperationException($"JwtSecret must be at least 32 characters long. Current length: {securitySettings.JwtSecret.Length}");

            if (string.IsNullOrWhiteSpace(securitySettings.JwtIssuer))
                throw new InvalidOperationException("JwtIssuer is required but not configured.");

            if (string.IsNullOrWhiteSpace(securitySettings.JwtAudience))
                throw new InvalidOperationException("JwtAudience is required but not configured.");

            return securitySettings;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"❌ Configuration Error: Failed to load Security settings.\n" +
                $"🔧 Fix: Ensure your appsettings.json contains a valid 'App:Security' section with JwtSecret, JwtIssuer, and JwtAudience.\n" +
                $"💡 Original error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets any configuration value safely
    /// </summary>
    public string GetRequiredString(string key)
    {
        return _configuration.GetRequiredValue(key);
    }

    /// <summary>
    /// Gets any configuration value safely with type conversion
    /// </summary>
    public T GetRequiredValue<T>(string key)
    {
        return _configuration.GetRequiredValue<T>(key);
    }

    /// <summary>
    /// Gets connection string safely
    /// </summary>
    public string GetConnectionString(string name)
    {
        var connectionString = _configuration.GetConnectionString(name);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"❌ Configuration Error: Connection string '{name}' is not configured.\n" +
                $"🔧 Fix: Add the following to your appsettings.json:\n" +
                $"   \"ConnectionStrings\": {{\n" +
                $"     \"{name}\": \"Data Source=server;Initial Catalog=database;...\"\n" +
                $"   }}");
        }

        return connectionString;
    }
}
