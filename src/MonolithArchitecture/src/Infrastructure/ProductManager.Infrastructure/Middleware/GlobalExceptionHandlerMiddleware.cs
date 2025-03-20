using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductManager.Infrastructure.Logging;
using ProductManager.Shared.Exceptions;
namespace ProductManager.Infrastructure.Middleware;

/// <summary>
///     Show a custom error message when an exception is thrown.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly GlobalExceptionHandlerMiddlewareOptions _options;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
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
            var response = context.Response;
            response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Detail = GetErrorMessage(ex)
            };

            problemDetails.Extensions.Add("message", GetErrorMessage(ex));
            if (Activity.Current != null)
            {
                problemDetails.Extensions.Add("traceId", Activity.Current.GetTraceId());
            }

            switch (ex)
            {
                case NotFoundException:
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    problemDetails.Title = "Not Found";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
                    break;

                case ValidationException:
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Bad Request";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
                    break;

                case UnauthorizedAccessException:
                    problemDetails.Status = (int)HttpStatusCode.Forbidden;// 403 Forbidden
                    problemDetails.Title = "Access Denied";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
                    break;

                case HttpRequestException { StatusCode: HttpStatusCode.Unauthorized }:
                    problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Title = "Unauthorized";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
                    break;

                case HttpRequestException { StatusCode: HttpStatusCode.RequestTimeout }:
                    problemDetails.Status = (int)HttpStatusCode.RequestTimeout;
                    problemDetails.Title = "Request Timeout";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.7";
                    break;

                case HttpRequestException:
                    problemDetails.Status = (int)HttpStatusCode.BadGateway;
                    problemDetails.Title = "Oracle Bad Gateway";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.3";
                    break;

                case OperationCanceledException:
                    problemDetails.Status = (int)HttpStatusCode.RequestTimeout;
                    problemDetails.Title = "Operation Canceled";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.7";
                    break;

                case TimeoutException:
                    problemDetails.Status = (int)HttpStatusCode.GatewayTimeout;
                    problemDetails.Title = "Request Timeout";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.5";
                    break;

                case DbUpdateException:
                    problemDetails.Status = (int)HttpStatusCode.Conflict;// 409 Conflict
                    problemDetails.Title = "Database Update Error";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8";
                    break;

                case SqlException:
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Title = "Database Error";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
                    break;

                default:
                    _logger.LogError(ex, "[{Ticks}-{ThreadId}]", DateTime.UtcNow.Ticks, Environment.CurrentManagedThreadId);
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Title = "Internal Server Error";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
                    break;
            }

            response.StatusCode = problemDetails.Status.Value;

            var result = JsonSerializer.Serialize(problemDetails);
            await response.WriteAsync(result);
        }
    }

    private static string GetErrorMessage(Exception ex) => ex.Message;
    // if (ex is ValidationException)
    // {
    //     return ex.Message;
    // }
    //
    // return _options.DetailLevel switch
    // {
    //     GlobalExceptionDetailLevel.None => "An internal exception has occurred.",
    //     GlobalExceptionDetailLevel.Message => ex.Message,
    //     GlobalExceptionDetailLevel.StackTrace => ex.StackTrace,
    //     GlobalExceptionDetailLevel.ToString => ex.ToString(),
    //     _ => "An internal exception has occurred."
    // };
}
public class GlobalExceptionHandlerMiddlewareOptions
{
    public GlobalExceptionDetailLevel DetailLevel { get; set; }
}
public enum GlobalExceptionDetailLevel
{
    None,
    Message,
    StackTrace,
    ToString,
    Throw
}
