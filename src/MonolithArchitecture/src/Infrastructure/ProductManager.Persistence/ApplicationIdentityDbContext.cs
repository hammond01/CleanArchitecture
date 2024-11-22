namespace ProductManager.Persistence;

public class ApplicationIdentityDbContext : DbContext
{
    public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
}
