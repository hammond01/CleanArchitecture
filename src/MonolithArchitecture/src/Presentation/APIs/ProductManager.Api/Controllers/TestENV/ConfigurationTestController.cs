using Microsoft.AspNetCore.Mvc;
using ProductManager.Infrastructure.Configuration;

namespace ProductManager.Api.Controllers.TestENV;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationTestController : ControllerBase
{
    private readonly SafeConfigurationAccessor _configAccessor;

    public ConfigurationTestController(SafeConfigurationAccessor configAccessor)
    {
        _configAccessor = configAccessor;
    }

    /// <summary>
    /// Test endpoint to demonstrate safe configuration access
    /// </summary>
    [HttpGet("test")]
    public IActionResult TestConfiguration()
    {
        try
        {
            // Test getting AppSettings
            var appSettings = _configAccessor.GetAppSettings();

            // Test getting Database settings
            var dbSettings = _configAccessor.GetDatabaseSettings();

            // Test getting Security settings
            var securitySettings = _configAccessor.GetSecuritySettings();

            return Ok(new
            {
                Success = true,
                Message = "✅ All configuration loaded successfully!",
                AppName = appSettings.Name,
                Version = appSettings.Version,
                Environment = appSettings.Environment,
                DatabaseConfigured = !string.IsNullOrEmpty(dbSettings.DefaultConnection),
                SecurityConfigured = !string.IsNullOrEmpty(securitySettings.JwtSecret),
                JwtSecretLength = securitySettings.JwtSecret?.Length ?? 0
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Success = false,
                Error = ex.Message,
                Type = ex.GetType().Name
            });
        }
    }

    /// <summary>
    /// Test endpoint to demonstrate what happens when configuration is missing
    /// </summary>
    [HttpGet("test-missing")]
    public IActionResult TestMissingConfiguration()
    {
        try
        {
            // This will throw a detailed error if the key doesn't exist
            var missingValue = _configAccessor.GetRequiredString("NonExistent:Key");

            return Ok(new { Value = missingValue });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Success = false,
                Error = ex.Message,
                Type = ex.GetType().Name,
                Note = "This is expected - demonstrating detailed error messages"
            });
        }
    }
}
