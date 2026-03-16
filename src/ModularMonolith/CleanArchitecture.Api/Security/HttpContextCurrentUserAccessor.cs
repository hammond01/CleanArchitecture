using BuildingBlocks.Application.Security;
using System.Security.Claims;

namespace CleanArchitecture.Api.Security;

/// <summary>
/// Resolves the current user from the active HTTP context.
/// </summary>
public sealed class HttpContextCurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? UserName => _httpContextAccessor.HttpContext?.User.Identity?.Name;

    public string? RequestMethod => _httpContextAccessor.HttpContext?.Request.Method;

    public string? RequestPath => _httpContextAccessor.HttpContext?.Request.Path.Value;

    public string? QueryString => _httpContextAccessor.HttpContext?.Request.QueryString.Value;

    public string? TraceIdentifier => _httpContextAccessor.HttpContext?.TraceIdentifier;
}
