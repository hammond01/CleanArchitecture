using Microsoft.AspNetCore.Identity;
using ProductManager.Domain.Entities.Identity;
using ProductManager.Domain.Identity;
namespace ProductManager.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        => new PasswordHasher<User>().VerifyHashedPassword(user, hashedPassword, providedPassword) !=
           PasswordVerificationResult.Failed;
}
