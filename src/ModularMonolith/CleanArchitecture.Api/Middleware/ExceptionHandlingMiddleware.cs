using BuildingBlocks.Api.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CleanArchitecture.Api.Middleware;

/// <summary>
/// Exception handling middleware for consistent error responses
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unhandled exception has occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var statusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            ValidationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var detail = statusCode == StatusCodes.Status500InternalServerError
            ? "An internal server error occurred"
            : exception.Message;

        var problemDetails = CreateProblemDetails(context, exception, statusCode, detail);

        return context.Response.WriteAsJsonAsync(
            problemDetails,
            options: null,
            contentType: "application/problem+json");
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext context,
        Exception exception,
        int statusCode,
        string detail)
    {
        var validationErrors = exception is ValidationException validationException
            ? validationException.Errors.Select(x => x.ErrorMessage)
            : null;

        return ApiProblemDetailsFactory.Create(
            context,
            statusCode,
            GetTitle(statusCode),
            detail,
            validationErrors);
    }

    private static string GetTitle(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status500InternalServerError => "Internal Server Error",
            _ => "Request Failed"
        };
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
