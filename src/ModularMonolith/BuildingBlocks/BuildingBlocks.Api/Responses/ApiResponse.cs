using System.Runtime.Serialization;

namespace BuildingBlocks.Api.Responses;

/// <summary>
/// Standard API response wrapper
/// </summary>
/// <typeparam name="T">Response data type</typeparam>
[Serializable]
[DataContract]
public class ApiResponse<T>
{
    [DataMember]
    public bool Success { get; set; }

    [DataMember]
    public int StatusCode { get; set; }

    public bool IsSuccessStatusCode => StatusCode is >= 200 and < 300;

    [DataMember]
    public string Message { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false)]
    public T? Data { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string? Error { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = 200,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResponse(string error, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = "Error",
            Error = error
        };
    }
}

/// <summary>
/// API response without data
/// </summary>
[Serializable]
[DataContract]
public class ApiResponse : ApiResponse<object>
{
    public ApiResponse()
    {
    }

    public ApiResponse(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
        Success = IsSuccessStatusCode;
    }

    public ApiResponse(int statusCode, string message, object? data)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;
        Success = IsSuccessStatusCode;
    }

    public static ApiResponse CreateSuccess(string message = "Success")
    {
        return new ApiResponse
        {
            Success = true,
            StatusCode = 200,
            Message = message
        };
    }

    public static ApiResponse CreateError(string error, int statusCode = 400)
    {
        return new ApiResponse
        {
            Success = false,
            StatusCode = statusCode,
            Message = "Error",
            Error = error
        };
    }
}
