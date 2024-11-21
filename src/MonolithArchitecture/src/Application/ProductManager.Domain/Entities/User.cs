namespace ProductManager.Domain.Entities;

public class User : Entity<Guid>
{
    public string UserName { get; set; } = string.Empty;

    public string NormalizedUserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string NormalizedEmail { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public string ConcurrencyStamp { get; set; } = string.Empty;

    public string SecurityStamp { get; set; } = string.Empty;

    public bool LockoutEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public int AccessFailedCount { get; set; }

    public string Auth0UserId { get; set; } = string.Empty;

    public string AzureAdB2CUserId { get; set; } = string.Empty;
}
