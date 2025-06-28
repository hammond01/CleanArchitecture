using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DateTimes;
namespace ProductManager.Persistence.Services;

/// <summary>
///     Implementation of IActionLogService for logging to database
/// </summary>
public class ActionLogService : IActionLogService
{
    private readonly ApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ActionLogService> _logger;

    public ActionLogService(ApplicationDbContext context, ILogger<ActionLogService> logger, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }
    public async Task LogActionAsync(ActionLogRequest request)
    {
        try
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.UserId ?? "Anonymous",
                CreatedDateTime = _dateTimeProvider.OffsetNow,
                Action = request.ActionName,
                ObjectId = request.Description ?? "API_ACTION",
                Log = JsonSerializer.Serialize(new
                {
                    request.ActionName,
                    request.Description,
                    request.ClientIpAddress,
                    request.UserAgent,
                    request.ExecutionTimeMs,
                    request.IsSuccess,
                    request.ErrorMessage,
                    request.StatusCode,
                    request.RequestParameters,
                    request.ResponseData
                })
            };

            _context.Set<AuditLog>().Add(auditLog);
            await _context.SaveChangesAsync();

            _logger.LogInformation("📊 Action logged: {ActionName} for user: {UserId} - Success: {IsSuccess} ({ExecutionTime}ms)",
            request.ActionName, request.UserId ?? "Anonymous", request.IsSuccess, request.ExecutionTimeMs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to log action: {ActionName}", request.ActionName);
        }
    }
    public async Task LogApiRequestAsync(string method, string path, int statusCode, long responseTimeMs,
        string? userId = null, string? clientIp = null)
    {
        try
        {
            // Skip logging for anonymous users to avoid foreign key constraint issues
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogDebug("📊 Skipping API log for anonymous user: {Method} {Path}", method, path);
                return;
            }

            // Only log for authenticated users
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogWarning("⚠️ Invalid UserId format for API logging: {UserId}", userId);
                return;
            }

            var apiLog = new ApiLogItem
            {
                Method = method,
                Path = path,
                StatusCode = statusCode,
                ResponseMillis = responseTimeMs,
                RequestTime = _dateTimeProvider.OffsetNow.DateTime,
                IpAddress = clientIp ?? "Unknown",
                QueryString = string.Empty,
                RequestBody = string.Empty,
                ResponseBody = string.Empty,
                UserId = parsedUserId,
                ApplicationUserId = parsedUserId
            };

            _context.Set<ApiLogItem>().Add(apiLog);
            await _context.SaveChangesAsync();

            _logger.LogInformation("📊 API request logged: {Method} {Path} - {StatusCode} ({ResponseTime}ms)",
            method, path, statusCode, responseTimeMs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to log API request: {Method} {Path}", method, path);
        }
    }
}
