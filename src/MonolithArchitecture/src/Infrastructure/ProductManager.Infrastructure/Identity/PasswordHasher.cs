namespace ProductManager.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    public bool VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        => new PasswordHasher<ApplicationUser>().VerifyHashedPassword(user, hashedPassword, providedPassword) !=
           PasswordVerificationResult.Failed;
}
