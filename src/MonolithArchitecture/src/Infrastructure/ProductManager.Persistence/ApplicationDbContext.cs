using ProductManager.Domain.Entities.Identity;
namespace ProductManager.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,
            IdentityUserToken<Guid>>(options),
        IUnitOfWork, IDataProtectionKeyContext
{
    private IDbContextTransaction _dbContextTransaction = null!;
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public async Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        _dbContextTransaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        return _dbContextTransaction;
    }
    public async Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        string? lockName = null,
        CancellationToken cancellationToken = default)
    {
        _dbContextTransaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        return _dbContextTransaction;
    }
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        => await _dbContextTransaction.CommitAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ConfigureIdentityTableNames(modelBuilder);
    }

    private static void ConfigureIdentityTableNames(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasMany(e => e.Roles)
            .WithMany(e => e.Users).UsingEntity<UserRole>(
            configureRight: l => l.HasOne<Role>().WithMany().HasForeignKey(e => e.RoleId),
            configureLeft: r => r.HasOne<User>().WithMany().HasForeignKey(e => e.UserId)
            );

        builder.Entity<UserRole>()
            .ToTable("UserRoles");

        builder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("UserLogins");

        builder.Entity<IdentityUserToken<Guid>>()
            .ToTable("UserTokens");

        builder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("RoleClaims");

        builder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("UserClaims");

        builder.Entity<User>()
            .ToTable("Users");

        builder.Entity<Role>()
            .ToTable("Roles");
    }
}
