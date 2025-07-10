using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductManager.Infrastructure.Configuration;

namespace ProductManager.Api.Controllers.TestENV;

/// <summary>
/// Controller để test AppSettings configuration
/// </summary>
[Route("api/v{version:apiVersion}/appsettings-test")]
[ApiController]
public class AppSettingsTestController : ControllerBase
{
    private readonly AppSettings _appSettings;
    private readonly DatabaseSettings _databaseSettings;
    private readonly SecuritySettings _securitySettings;

    public AppSettingsTestController(
        IOptions<AppSettings> appSettings,
        IOptions<DatabaseSettings> databaseSettings,
        IOptions<SecuritySettings> securitySettings)
    {
        _appSettings = appSettings.Value;
        _databaseSettings = databaseSettings.Value;
        _securitySettings = securitySettings.Value;
    }

    /// <summary>
    /// Kiểm tra AppSettings có hoạt động không
    /// </summary>
    [HttpGet("status")]
    public ActionResult<object> GetAppSettingsStatus()
    {
        return Ok(new
        {
            App = new
            {
                _appSettings.Name,
                _appSettings.Version,
                _appSettings.Environment
            },
            Database = new
            {
                HasDefaultConnection = !string.IsNullOrEmpty(_databaseSettings.DefaultConnection),
                HasIdentityConnection = !string.IsNullOrEmpty(_databaseSettings.IdentityConnection),
                _databaseSettings.CommandTimeout,
                _databaseSettings.MaxRetryCount,
                _databaseSettings.EnableSensitiveDataLogging
            },
            Security = new
            {
                HasJwtSecret = !string.IsNullOrEmpty(_securitySettings.JwtSecret),
                HasJwtIssuer = !string.IsNullOrEmpty(_securitySettings.JwtIssuer),
                HasJwtAudience = !string.IsNullOrEmpty(_securitySettings.JwtAudience),
                _securitySettings.JwtExpirationMinutes,
                _securitySettings.RefreshTokenExpirationDays,
                _securitySettings.RequireHttps,
                AllowedOriginsCount = _securitySettings.AllowedOrigins?.Length ?? 0
            },
            IsConfigured = !string.IsNullOrEmpty(_databaseSettings.DefaultConnection) &&
                          !string.IsNullOrEmpty(_securitySettings.JwtSecret)
        });
    }

    /// <summary>
    /// Lấy thông tin kết nối cơ sở dữ liệu (an toàn)
    /// </summary>
    [HttpGet("database-info")]
    public ActionResult<object> GetDatabaseInfo()
    {
        return Ok(new
        {
            HasDefaultConnection = !string.IsNullOrEmpty(_databaseSettings.DefaultConnection),
            HasIdentityConnection = !string.IsNullOrEmpty(_databaseSettings.IdentityConnection),
            DefaultConnectionPreview = _databaseSettings.DefaultConnection?.Length > 20
                ? _databaseSettings.DefaultConnection[..20] + "..."
                : _databaseSettings.DefaultConnection,
            Settings = new
            {
                _databaseSettings.CommandTimeout,
                _databaseSettings.MaxRetryCount,
                _databaseSettings.EnableSensitiveDataLogging
            }
        });
    }
}
