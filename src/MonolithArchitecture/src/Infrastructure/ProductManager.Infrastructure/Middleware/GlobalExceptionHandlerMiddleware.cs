using System.Text.Json.Serialization;
using ProductManager.Shared.Logging;
namespace ProductManager.Infrastructure.Middleware;

/// <summary>
///     Show a custom error message when an exception is thrown.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;
    private readonly GlobalExceptionHandlerMiddlewareOptions _options;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger logger,
        GlobalExceptionHandlerMiddlewareOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var statusCode = GetStatusCode(exception);
        var errorResponse = new ApiResponseException
        {
            Success = false, StatusCode = statusCode, Message = GetErrorMessage(exception), Errors = GetErrors(exception)
        };

        // Log exception with structured data
        _logger.LogError("An error occurred: {Message} | Path: {Path} | Method: {Method} | StatusCode: {StatusCode}",
        exception.Message,
        context.Request.Path,
        context.Request.Method,
        statusCode
        );

        response.StatusCode = statusCode;
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

    private static int GetStatusCode(Exception ex) => ex switch
    {
        ValidationException => (int)HttpStatusCode.BadRequest,
        NotFoundException => (int)HttpStatusCode.NotFound,
        UnauthorizedException => (int)HttpStatusCode.Unauthorized,
        _ => (int)HttpStatusCode.InternalServerError
    };

    private static string GetErrorMessage(Exception ex) => ex switch
    {
        ValidationException => "Validation Error",
        NotFoundException => "Not Found",
        UnauthorizedException => "Unauthorized",
        _ => "Internal Server Error"
    };

    private List<string> GetErrors(Exception ex)
    {
        var errors = new List<string>();

        switch (ex)
        {
            case ValidationException validationEx:
                errors.Add(validationEx.Message);
                break;
            case NotFoundException notFoundEx:
                errors.Add(notFoundEx.Message);
                break;
            case UnauthorizedException unauthorizedEx:
                errors.Add(unauthorizedEx.Message);
                break;
            default:
                if (_options.DetailLevel != GlobalExceptionDetailLevel.None)
                {
                    errors.Add(ex.Message);
                }
                break;
        }

        return errors;
    }
}
public class GlobalExceptionHandlerMiddlewareOptions
{
    public GlobalExceptionDetailLevel DetailLevel { get; set; } = GlobalExceptionDetailLevel.Message;
}
public enum GlobalExceptionDetailLevel
{
    None,
    Message
}
public class ApiResponseException
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("isSuccessStatusCode")]
    public bool IsSuccessStatusCode => StatusCode is >= 200 and < 300;
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = [];
}
