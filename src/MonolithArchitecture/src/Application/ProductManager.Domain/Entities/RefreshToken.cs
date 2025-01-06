namespace ProductManager.Domain.Entities;

[Table("RefreshToken")]
public class RefreshToken : Entity<string>
{
    [StringLength(250)]
    public string Token { get; set; } = default!;
    [StringLength(250)]
    public Guid UserId { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
}
