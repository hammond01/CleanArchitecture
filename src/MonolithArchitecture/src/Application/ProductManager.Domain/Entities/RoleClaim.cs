namespace ProductManager.Domain.Entities;

public class RoleClaim : Entity<Guid>
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public Role Role { get; set; } = null!;
}
