using ProductManager.Domain.Entities.Identity;
namespace ProductManager.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,
            IdentityUserToken<Guid>>(options),
        IUnitOfWork, IDataProtectionKeyContext
{
    private IDbContextTransaction _dbContextTransaction = null!;

    public virtual DbSet<Categories> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Products> Products { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Shipper> Shippers { get; set; }

    public virtual DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }

    public virtual DbSet<Suppliers> Suppliers { get; set; }

    public virtual DbSet<Territory> Territories { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<OutboxEvent> OutboxEvents { get; set; }

    public virtual DbSet<ApiLogItem> ApiLogs { get; set; }

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

        modelBuilder.Entity<EmployeeTerritory>()
            .HasKey(et => new
            {
                et.EmployeeId, et.TerritoryId
            });

        modelBuilder.Entity<EmployeeTerritory>()
            .HasOne(et => et.Employee)
            .WithMany(e => e.EmployeeTerritories)
            .HasForeignKey(et => et.EmployeeId);

        modelBuilder.Entity<EmployeeTerritory>()
            .HasOne(et => et.Territory)
            .WithMany(t => t.EmployeeTerritories)
            .HasForeignKey(et => et.TerritoryId);

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
