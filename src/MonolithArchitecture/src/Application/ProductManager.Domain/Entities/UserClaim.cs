namespace ProductManager.Domain.Entities;

public class UserClaim : Entity<Guid>
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public User User { get; set; } = default!;
}
