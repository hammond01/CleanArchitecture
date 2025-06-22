using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DateTimes;

namespace ProductManager.Persistence.Services;

/// <summary>
/// Implementation of IActionLogService for logging to database
/// </summary>
public class ActionLogService : IActionLogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ActionLogService> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

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
                Log = System.Text.Json.JsonSerializer.Serialize(new
                {
                    ActionName = request.ActionName,
                    Description = request.Description,
                    ClientIpAddress = request.ClientIpAddress,
                    UserAgent = request.UserAgent,
                    ExecutionTimeMs = request.ExecutionTimeMs,
                    IsSuccess = request.IsSuccess,
                    ErrorMessage = request.ErrorMessage,
                    StatusCode = request.StatusCode,
                    RequestParameters = request.RequestParameters,
                    ResponseData = request.ResponseData
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
        string? userId = null, string? clientIp = null, string? userAgent = null,
        int? requestSize = null, int? responseSize = null)
    {
        try
        {
            // Anonymous user GUID for required UserId field
            var anonymousUserId = new Guid("00000000-0000-0000-0000-000000000001");
            var parsedUserId = string.IsNullOrEmpty(userId) ? anonymousUserId :
                              Guid.TryParse(userId, out var guid) ? guid : anonymousUserId; var apiLog = new ApiLogItem
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
                                  UserId = parsedUserId, // Required field
                                  ApplicationUserId = string.IsNullOrEmpty(userId) ? null : Guid.TryParse(userId, out var appGuid) ? appGuid : null
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
