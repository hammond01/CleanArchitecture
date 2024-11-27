namespace ProductManager.Domain.Entities;

[Table("RefreshToken")]
public class RefreshToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [StringLength(250)]
    public string Token { get; set; } = default!;
    [StringLength(250)]
    public Guid UserId { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
}
