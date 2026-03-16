namespace BuildingBlocks.Shared.Constants;

/// <summary>
/// Common API response constants
/// </summary>
public static class ApiResponseMessage
{
    public const string Success = "Operation completed successfully";
    public const string Created = "Resource created successfully";
    public const string Updated = "Resource updated successfully";
    public const string Deleted = "Resource deleted successfully";
    public const string BadRequest = "Invalid request data";
    public const string Unauthorized = "Unauthorized access";
    public const string Forbidden = "Access forbidden";
    public const string NotFound = "Resource not found";
    public const string InternalError = "Internal server error";
    public const string ValidationError = "Validation failed";
}
