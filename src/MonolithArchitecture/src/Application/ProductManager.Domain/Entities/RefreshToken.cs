namespace ProductManager.Domain.Entities;

[Table("RefreshToken")]
public class RefreshToken : Entity<string>
{
    [StringLength(250)]
    public string Token { get; set; } = default!;
    [StringLength(250)]
    public Guid UserId { get; set; }
    public DateTimeOffset Expires { get; set; }
    public bool IsExpired => DateTimeOffset.UtcNow >= Expires;
    public DateTimeOffset? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
}
