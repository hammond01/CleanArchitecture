namespace ProductManager.Domain.Entities;

public class User : Entity<Guid>
{
    [StringLength(250)]
    public string UserName { get; set; } = string.Empty;
    [StringLength(250)]
    public string NormalizedUserName { get; set; } = string.Empty;
    [StringLength(250)]
    public string Email { get; set; } = string.Empty;
    [StringLength(250)]
    public string NormalizedEmail { get; set; } = string.Empty;
    [StringLength(250)]
    public bool EmailConfirmed { get; set; }
    [StringLength(250)]
    public string PasswordHash { get; set; } = string.Empty;
    [StringLength(250)]
    public string PhoneNumber { get; set; } = string.Empty;

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }
    [StringLength(250)]
    public string ConcurrencyStamp { get; set; } = string.Empty;
    [StringLength(250)]
    public string SecurityStamp { get; set; } = string.Empty;

    public bool LockoutEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public int AccessFailedCount { get; set; }
    [StringLength(250)]
    public string Auth0UserId { get; set; } = string.Empty;
    [StringLength(250)]
    public string AzureAdB2CUserId { get; set; } = string.Empty;
}
