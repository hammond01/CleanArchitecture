namespace ProductManager.Domain.Common;

/// <summary>
///     Request model for logging actions
/// </summary>
public class ActionLogRequest
{
    public string ActionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? UserId { get; set; }
    public string ClientIpAddress { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public int ExecutionTimeMs { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }
    public string? RequestParameters { get; set; }
    public string? ResponseData { get; set; }
}
/// <summary>
///     Service for logging user actions and API requests
/// </summary>
public interface IActionLogService
{
    /// <summary>
    ///     Log a user action to the database
    /// </summary>
    Task LogActionAsync(ActionLogRequest request);

    /// <summary>
    ///     Log an API request to the database
    /// </summary>
    Task LogApiRequestAsync(string method, string path, int statusCode, long responseTimeMs,
        string? userId = null, string? clientIp = null);
}
/// <summary>
///     Attribute to mark actions that should be logged
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class LogActionAttribute : Attribute
{

    public LogActionAttribute(string description)
    {
        Description = description;
    }
    public string Description { get; }
}
