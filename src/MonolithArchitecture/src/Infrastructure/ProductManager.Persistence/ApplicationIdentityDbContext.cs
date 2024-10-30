namespace ProductManager.Persistence;

public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
}
