namespace BuildingBlocks.Shared.DTOs;

/// <summary>
/// Standard API response wrapper
/// </summary>
public class ApiResponse
{
    public ApiResponse()
    {
    }

    public ApiResponse(int statusCode, string message, object? result = null)
    {
        StatusCode = statusCode;
        Message = message;
        Result = result;
        Timestamp = DateTimeOffset.UtcNow;
    }

    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public object? Result { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public List<string>? Errors { get; set; }
}

/// <summary>
/// Generic API response wrapper
/// </summary>
public class ApiResponse<T>
{
    public ApiResponse()
    {
    }

    public ApiResponse(int statusCode, string message, T? result = default)
    {
        StatusCode = statusCode;
        Message = message;
        Result = result;
        Timestamp = DateTimeOffset.UtcNow;
    }

    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public T? Result { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public List<string>? Errors { get; set; }
}
