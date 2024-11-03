using ProductManager.Domain.Entities;
namespace ProductManager.Domain.Identity;

public interface IPasswordHasher
{
    bool VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword);
}
