using ProductManager.Domain.Entities.Identity;

namespace ProductManager.Domain.Identity;

public interface IPasswordHasher
{
    bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
}
