using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Domain.Entities;

/// <summary>
/// RefreshToken entity for JWT token management
/// </summary>
public class RefreshToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [StringLength(500)]
    public string Token { get; set; } = default!;

    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public DateTimeOffset Expires { get; set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= Expires;

    public DateTimeOffset? Revoked { get; set; }

    public bool IsActive => Revoked == null && !IsExpired;

    public DateTimeOffset CreatedDateTime { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedDateTime { get; set; }
}
