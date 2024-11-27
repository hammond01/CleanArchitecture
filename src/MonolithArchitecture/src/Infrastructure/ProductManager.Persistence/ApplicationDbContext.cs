namespace ProductManager.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork, IDataProtectionKeyContext
{
    private IDbContextTransaction _dbContextTransaction = null!;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
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

    public virtual DbSet<User> Users { get; set; }

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
    }
}
