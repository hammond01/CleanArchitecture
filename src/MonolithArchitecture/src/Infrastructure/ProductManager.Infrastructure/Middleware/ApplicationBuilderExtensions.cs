namespace ProductManager.Infrastructure.Middleware;

/// <summary>
///     Register the global exception handler middleware.
/// </summary>
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder app,
        GlobalExceptionHandlerMiddlewareOptions options = default!)
    {
        options ??= new GlobalExceptionHandlerMiddlewareOptions();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>(options);
        return app;
    }

    public static IApplicationBuilder UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder app,
        Action<GlobalExceptionHandlerMiddlewareOptions> configureOptions)
    {
        var options = new GlobalExceptionHandlerMiddlewareOptions();
        configureOptions(options);
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>(options);
        return app;
    }

    public static IApplicationBuilder UseLoggingStatusCodeMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<LoggingStatusCodeMiddleware>();
        return app;
    }
}
