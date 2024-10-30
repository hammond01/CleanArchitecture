using System.Security.Principal;
namespace ProductManager.Persistence.Extensions;

public static class Extensions
{
    public static string GetSubjectId(this IPrincipal principal) => principal.Identity!.GetSubjectId();

    public static string GetSubjectId(this IIdentity identity)
    {
        var id = identity as ClaimsIdentity;
        var claim = id!.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
        {
            throw new InvalidOperationException("sub claim is missing");
        }
        return claim.Value;
    }

    public static string GetDisplayName(this ClaimsPrincipal principal)
    {
        var name = principal.Identity!.Name;
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        var sub = principal.FindFirst(ClaimTypes.NameIdentifier);
        return sub != null ? sub.Value : string.Empty;

    }

    public static Guid GetUserId(this ClaimsPrincipal user)
        => user.Identity!.IsAuthenticated ? new Guid(user.GetSubjectId()) : new Guid();

    public static string GetErrors(this IdentityResult result) => string.Join("\n", result.Errors.Select(i => i.Description));
}
