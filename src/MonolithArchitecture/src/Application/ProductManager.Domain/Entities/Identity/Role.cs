using Microsoft.AspNetCore.Identity;

namespace ProductManager.Domain.Entities.Identity;

public class Role : IdentityRole<Guid>
{
    public Role()
    {
    }
    public Role(string roleName) : base(roleName) { }
    public IList<User> Users { get; set; } = [];
}
