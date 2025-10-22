using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Domain.Entities;

/// <summary>
/// User claims for additional user properties
/// </summary>
public class UserClaim : IdentityUserClaim<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}
