using System.ComponentModel.DataAnnotations;
using BuildingBlocks.Domain.Entities;

namespace Identity.Domain.Entities;

/// <summary>
/// User entity for authentication and identity management
/// </summary>
public class User : Entity<Guid>
{
    [StringLength(250)]
    public string UserName { get; set; } = null!;

    [StringLength(250)]
    public string Email { get; set; } = null!;

    [StringLength(500)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(250)]
    public string FirstName { get; set; } = null!;

    [StringLength(250)]
    public string LastName { get; set; } = null!;

    public bool IsEmailConfirmed { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public string? EmailConfirmationToken { get; set; }

    public DateTimeOffset? EmailConfirmedDateTime { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTimeOffset? PasswordResetTokenExpires { get; set; }

    public int LoginAttempts { get; set; } = 0;

    public DateTimeOffset? LastLoginDateTime { get; set; }

    public DateTimeOffset? LockoutEndDateTime { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
