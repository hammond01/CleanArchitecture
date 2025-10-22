using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Domain.Entities;

/// <summary>
/// Many-to-many relationship between users and roles
/// </summary>
public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid? AssignedBy { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationRole Role { get; set; } = null!;
}
