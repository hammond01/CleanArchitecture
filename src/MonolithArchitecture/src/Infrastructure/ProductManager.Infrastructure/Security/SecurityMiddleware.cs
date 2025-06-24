// Advanced security middleware and configurations
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace ProductManager.Infrastructure.Security;

/// <summary>
/// Security headers middleware for enhanced API security
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers
        var headers = context.Response.Headers;

        // Prevent XSS attacks
        headers["X-XSS-Protection"] = "1; mode=block";

        // Prevent MIME sniffing
        headers["X-Content-Type-Options"] = "nosniff";

        // Prevent clickjacking
        headers["X-Frame-Options"] = "DENY";

        // Strict Transport Security
        headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";

        // Content Security Policy
        headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'";

        // Referrer Policy
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Permissions Policy
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

        // Remove server information
        headers.Remove("Server");
        headers["Server"] = "ProductManager-API";

        await _next(context);
    }
}

/// <summary>
/// Request signing middleware for API integrity verification
/// </summary>
public class RequestSigningMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestSigningMiddleware> _logger;
    private readonly RequestSigningOptions _options;

    public RequestSigningMiddleware(
        RequestDelegate next,
        ILogger<RequestSigningMiddleware> logger,
        RequestSigningOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_options.RequireSignedRequests && !IsSignatureValid(context))
        {
            _logger.LogWarning("🚫 Invalid request signature from {IP}",
                context.Connection.RemoteIpAddress);

            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid request signature");
            return;
        }

        await _next(context);
    }

    private bool IsSignatureValid(HttpContext context)
    {
        // Skip signature validation for certain paths
        var path = context.Request.Path.Value?.ToLower();
        if (path?.Contains("/health") == true || path?.Contains("/swagger") == true)
            return true;

        var signature = context.Request.Headers["X-Signature"].FirstOrDefault();
        var timestamp = context.Request.Headers["X-Timestamp"].FirstOrDefault();

        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timestamp))
            return !_options.RequireSignedRequests;

        // Validate timestamp (prevent replay attacks)
        if (!long.TryParse(timestamp, out var timestampValue))
            return false;

        var requestTime = DateTimeOffset.FromUnixTimeSeconds(timestampValue);
        if (Math.Abs((DateTimeOffset.UtcNow - requestTime).TotalMinutes) > _options.MaxTimestampAge.TotalMinutes)
        {
            _logger.LogWarning("🕐 Request timestamp too old: {RequestTime}", requestTime);
            return false;
        }        // Build signature payload
        var method = context.Request.Method;
        var requestPath = context.Request.Path.Value;
        var query = context.Request.QueryString.Value;

        var payload = $"{method}|{requestPath}|{query}|{timestamp}";
        var expectedSignature = ComputeHmacSha256(payload, _options.SecretKey);

        return signature.Equals(expectedSignature, StringComparison.OrdinalIgnoreCase);
    }

    private static string ComputeHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }
}

public class RequestSigningOptions
{
    public bool RequireSignedRequests { get; set; } = false;
    public string SecretKey { get; set; } = string.Empty;
    public TimeSpan MaxTimestampAge { get; set; } = TimeSpan.FromMinutes(5);
}

/// <summary>
/// IP whitelist middleware for API access control
/// </summary>
public class IpWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpWhitelistMiddleware> _logger;
    private readonly IpWhitelistOptions _options;

    public IpWhitelistMiddleware(
        RequestDelegate next,
        ILogger<IpWhitelistMiddleware> logger,
        IpWhitelistOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_options.EnableWhitelist && !IsIpAllowed(context))
        {
            var clientIp = GetClientIpAddress(context);
            _logger.LogWarning("🚫 Access denied for IP: {IP}", clientIp);

            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access denied");
            return;
        }

        await _next(context);
    }

    private bool IsIpAllowed(HttpContext context)
    {
        var clientIp = GetClientIpAddress(context);

        // Allow localhost in development
        if (clientIp == "127.0.0.1" || clientIp == "::1")
            return true;

        return _options.AllowedIps.Contains(clientIp, StringComparer.OrdinalIgnoreCase);
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        return context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
               context.Request.Headers["X-Real-IP"].FirstOrDefault() ??
               context.Connection.RemoteIpAddress?.ToString() ??
               "Unknown";
    }
}

public class IpWhitelistOptions
{
    public bool EnableWhitelist { get; set; } = false;
    public List<string> AllowedIps { get; set; } = new();
}

/// <summary>
/// Security extensions for service registration
/// </summary>
public static class SecurityExtensions
{
    public static IServiceCollection AddAdvancedSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure request signing
        var signingOptions = configuration.GetSection("RequestSigning").Get<RequestSigningOptions>()
                           ?? new RequestSigningOptions();
        services.AddSingleton(signingOptions);

        // Configure IP whitelist
        var whitelistOptions = configuration.GetSection("IpWhitelist").Get<IpWhitelistOptions>()
                             ?? new IpWhitelistOptions();
        services.AddSingleton(whitelistOptions);

        return services;
    }

    public static IApplicationBuilder UseAdvancedSecurity(this IApplicationBuilder app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseMiddleware<IpWhitelistMiddleware>();
        app.UseMiddleware<RequestSigningMiddleware>();

        return app;
    }
}
