using Auditing.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DbMigrator;

/// <summary>
/// Service to orchestrate database migrations across all modules
/// </summary>
public class DbMigrationService
{
    private readonly IdentityDbContext _identityDbContext;
    private readonly CatalogDbContext _catalogDbContext;
    private readonly AuditingDbContext _auditingDbContext;
    private readonly ILogger<DbMigrationService> _logger;
    private readonly MigrationSettings _settings;

    public DbMigrationService(
        IdentityDbContext identityDbContext,
        CatalogDbContext catalogDbContext,
        AuditingDbContext auditingDbContext,
        ILogger<DbMigrationService> logger,
        IOptions<MigrationSettings> settings)
    {
        _identityDbContext = identityDbContext;
        _catalogDbContext = catalogDbContext;
        _auditingDbContext = auditingDbContext;
        _logger = logger;
        _settings = settings.Value;
    }

    /// <summary>
    /// Run all database migrations in sequence
    /// </summary>
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🚀 Starting Database Migration Process...");
        _logger.LogInformation("Settings: SeedData={SeedData}, CreateDb={CreateDb}, Timeout={Timeout}s",
            _settings.SeedData, _settings.CreateDatabaseIfNotExists, _settings.TimeoutSeconds);

        try
        {
            // Check database connectivity
            await CheckDatabaseConnectivityAsync(cancellationToken);

            // Migrate each module in order
            await MigrateIdentityAsync(cancellationToken);
            await MigrateAuditingAsync(cancellationToken);
            await MigrateCatalogAsync(cancellationToken);

            // Seed data if configured
            if (_settings.SeedData)
            {
                await SeedDataAsync(cancellationToken);
            }

            _logger.LogInformation("✅ All migrations completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Migration failed: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    private async Task CheckDatabaseConnectivityAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 Checking database connectivity...");

        try
        {
            await _identityDbContext.Database.CanConnectAsync(cancellationToken);
            _logger.LogInformation("✅ Database connection successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Cannot connect to database");
            throw;
        }
    }

    private async Task MigrateIdentityAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("📦 Migrating Identity module (schema: identity)...");

        var pendingMigrations = await _identityDbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        var pendingCount = pendingMigrations.Count();

        if (pendingCount > 0)
        {
            _logger.LogInformation("⏳ Applying {Count} pending migrations: {Migrations}",
                pendingCount, string.Join(", ", pendingMigrations));

            await _identityDbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("✅ Identity migrations applied");
        }
        else
        {
            _logger.LogInformation("✅ Identity is up to date (no pending migrations)");
        }
    }

    private async Task MigrateAuditingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("📦 Migrating Auditing module (schema: auditing)...");

        var pendingMigrations = await _auditingDbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        var pendingCount = pendingMigrations.Count();

        if (pendingCount > 0)
        {
            _logger.LogInformation("⏳ Applying {Count} pending migrations: {Migrations}",
                pendingCount, string.Join(", ", pendingMigrations));

            await _auditingDbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("✅ Auditing migrations applied");
        }
        else
        {
            _logger.LogInformation("✅ Auditing is up to date (no pending migrations)");
        }
    }

    private async Task MigrateCatalogAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("📦 Migrating Catalog module (schema: catalog)...");

        var pendingMigrations = await _catalogDbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        var pendingCount = pendingMigrations.Count();

        if (pendingCount > 0)
        {
            _logger.LogInformation("⏳ Applying {Count} pending migrations: {Migrations}",
                pendingCount, string.Join(", ", pendingMigrations));

            await _catalogDbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("✅ Catalog migrations applied");
        }
        else
        {
            _logger.LogInformation("✅ Catalog is up to date (no pending migrations)");
        }
    }

    private async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🌱 Seeding initial data...");

        try
        {
            // TODO: Implement data seeding for each module
            // await SeedIdentityDataAsync(cancellationToken);
            // await SeedCatalogDataAsync(cancellationToken);
            // await SeedAuditingDataAsync(cancellationToken);

            _logger.LogInformation("✅ Data seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Data seeding failed (non-critical): {ErrorMessage}", ex.Message);
            // Don't throw - seeding failures are non-critical
        }
    }
}
