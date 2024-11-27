namespace ProductManager.Domain.Entities.Identity;

public class User : IdentityUser<Guid>
{
    [MaxLength(64)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(64)]
    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public IList<Role> Roles { get; set; } = [];
}
