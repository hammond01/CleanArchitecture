namespace ProductManager.Domain.Entities;

public class Role : Entity<Guid>
{
    public virtual string Name { get; set; } = string.Empty;

    public virtual string NormalizedName { get; set; } = string.Empty;

    public virtual string ConcurrencyStamp { get; set; } = string.Empty;

    public IList<RoleClaim> Claims { get; set; } = new List<RoleClaim>();

    public IList<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
