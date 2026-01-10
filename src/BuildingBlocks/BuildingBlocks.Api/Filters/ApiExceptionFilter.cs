using BuildingBlocks.Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Api.Filters;

/// <summary>
/// Global exception filter for API controllers
/// </summary>
public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred: {Message}", context.Exception.Message);

        var response = context.Exception switch
        {
            ArgumentException => ApiResponse.ErrorResponse(context.Exception.Message, 400),
            UnauthorizedAccessException => ApiResponse.ErrorResponse("Unauthorized access", 401),
            KeyNotFoundException => ApiResponse.ErrorResponse("Resource not found", 404),
            InvalidOperationException => ApiResponse.ErrorResponse(context.Exception.Message, 400),
            _ => ApiResponse.ErrorResponse("An internal server error occurred", 500)
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };

        context.ExceptionHandled = true;
    }
}
