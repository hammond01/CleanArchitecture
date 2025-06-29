using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProductManager.Domain.Identity;
namespace ProductManager.Infrastructure.Identity;

public class CurrentWebUser : ICurrentUser
{
    private readonly IHttpContextAccessor _context;

    public CurrentWebUser(IHttpContextAccessor context)
    {
        _context = context;
    }

    public bool IsAuthenticated => _context.HttpContext?.User.Identity is { IsAuthenticated: true };
    public string UserName
    {
        get
        {
            var userName = _context.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
            return userName ?? string.Empty;
        }
    }

    public string UserId
    {
        get
        {
            var userId = _context.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? _context.HttpContext?.User.FindFirst("sub")?.Value;
            return userId ?? string.Empty;
        }
    }
}
