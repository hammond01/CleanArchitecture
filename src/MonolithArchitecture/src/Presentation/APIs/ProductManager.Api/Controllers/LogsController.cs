using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Common;
using ProductManager.Persistence;
namespace ProductManager.Api.Controllers;

/// <summary>
///     Controller for viewing logs stored in the database
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/logs")]
public class LogsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LogsController> _logger;

    public LogsController(ApplicationDbContext context, ILogger<LogsController> logger)
    {
        _context = context;
        _logger = logger;
    }
    /// <summary>
    ///     Get API logs with pagination
    /// </summary>
    [HttpGet("api")]
    [LogAction("View API logs")]
    public async Task<ActionResult<ApiResponse>> GetApiLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? userId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        _logger.LogInformation("📊 Retrieving API logs - Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var query = _context.ApiLogs.AsQueryable();

        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
        {
            query = query.Where(x => x.ApplicationUserId == userGuid);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.RequestTime >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.RequestTime <= toDate.Value);
        }

        var totalCount = await query.CountAsync();
        var logs = await query
            .OrderByDescending(x => x.RequestTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Id,
                x.Method,
                x.Path,
                x.StatusCode,
                ResponseTimeMs = x.ResponseMillis,
                UserId = x.ApplicationUserId,
                ClientIpAddress = x.IpAddress,
                Timestamp = x.RequestTime,
                x.QueryString,
                x.RequestBody,
                x.ResponseBody
            })
            .ToListAsync();

        var result = new
        {
            Data = logs,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };

        _logger.LogInformation("✅ Retrieved {Count} API logs out of {Total}", logs.Count, totalCount);
        return Ok(result);
    }
    /// <summary>
    ///     Get action logs with pagination
    /// </summary>
    [HttpGet("actions")]
    [LogAction("View action logs")]
    public async Task<ActionResult<ApiResponse>> GetActionLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? userId = null,
        [FromQuery] string? actionName = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        _logger.LogInformation("📊 Retrieving action logs - Page: {Page}, PageSize: {PageSize}", page, pageSize);

        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(x => x.UserId == userId);
        }

        if (!string.IsNullOrEmpty(actionName))
        {
            query = query.Where(x => x.Action.Contains(actionName));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.CreatedDateTime >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.CreatedDateTime <= toDate.Value);
        }

        var totalCount = await query.CountAsync();
        var logs = await query
            .OrderByDescending(x => x.CreatedDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Id,
                ActionName = x.Action,
                x.UserId,
                x.ObjectId,
                x.Log,
                Timestamp = x.CreatedDateTime
            })
            .ToListAsync();

        var result = new
        {
            Data = logs,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };

        _logger.LogInformation("✅ Retrieved {Count} action logs out of {Total}", logs.Count, totalCount);
        return Ok(result);
    }

    /// <summary>
    ///     Get log statistics
    /// </summary>
    [HttpGet("statistics")]
    [LogAction("View log statistics")]
    public async Task<ActionResult<ApiResponse>> GetLogStatistics([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
    {
        _logger.LogInformation("📈 Retrieving log statistics");

        var apiLogsQuery = _context.ApiLogs.AsQueryable();
        var actionLogsQuery = _context.AuditLogs.AsQueryable();

        if (fromDate.HasValue)
        {
            apiLogsQuery = apiLogsQuery.Where(x => x.RequestTime >= fromDate.Value);
            actionLogsQuery = actionLogsQuery.Where(x => x.CreatedDateTime >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            apiLogsQuery = apiLogsQuery.Where(x => x.RequestTime <= toDate.Value);
            actionLogsQuery = actionLogsQuery.Where(x => x.CreatedDateTime <= toDate.Value);
        }

        var apiLogStats = await apiLogsQuery
            .GroupBy(x => 1)
            .Select(g => new
            {
                TotalRequests = g.Count(),
                SuccessfulRequests = g.Count(x => x.StatusCode >= 200 && x.StatusCode < 300),
                ErrorRequests = g.Count(x => x.StatusCode >= 400),
                AverageResponseTime = g.Any() ? g.Average(x => (double)x.ResponseMillis) : 0.0
            })
            .FirstOrDefaultAsync();

        var actionLogStats = await actionLogsQuery
            .GroupBy(x => 1)
            .Select(g => new
            {
                TotalActions = g.Count()
            })
            .FirstOrDefaultAsync();

        var topEndpoints = await apiLogsQuery
            .GroupBy(x => new
            {
                x.Method,
                x.Path
            })
            .Select(g => new
            {
                Endpoint = $"{g.Key.Method} {g.Key.Path}",
                Count = g.Count(),
                AverageResponseTime = g.Any() ? g.Average(x => (double)x.ResponseMillis) : 0.0
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        var result = new
        {
            ApiLogs = apiLogStats ?? new
            {
                TotalRequests = 0,
                SuccessfulRequests = 0,
                ErrorRequests = 0,
                AverageResponseTime = 0.0
            },
            ActionLogs = actionLogStats ?? new
            {
                TotalActions = 0
            },
            TopEndpoints = topEndpoints
        };

        _logger.LogInformation("✅ Retrieved log statistics");
        return Ok(result);
    }
}
