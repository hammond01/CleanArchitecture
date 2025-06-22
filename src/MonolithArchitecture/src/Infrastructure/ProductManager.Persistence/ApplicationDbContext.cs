using System.Data;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Entities.Identity;
using ProductManager.Domain.Repositories;
namespace ProductManager.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,
            IdentityUserToken<Guid>>(options),
        IUnitOfWork, IDataProtectionKeyContext
{
    private IDbContextTransaction _dbContextTransaction = null!;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    // Business Entities
    public DbSet<Products> Products { get; set; } = null!;
    public DbSet<Categories> Categories { get; set; } = null!;
    public DbSet<Suppliers> Suppliers { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;

    // Logging Entities
    public DbSet<ApiLogItem> ApiLogs { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

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
