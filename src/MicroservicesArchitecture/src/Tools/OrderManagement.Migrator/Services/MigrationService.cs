using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderManagement.Migrator.Data;

namespace OrderManagement.Migrator.Services;

/// <summary>
/// Service for managing database migrations
/// </summary>
public class MigrationService
{
    private readonly OrderManagementDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(
        OrderManagementDbContext context,
        IConfiguration configuration,
        ILogger<MigrationService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Apply all pending migrations
    /// </summary>
    public async Task ApplyMigrationsAsync()
    {
        try
        {
            _logger.LogInformation("Starting database migration...");

            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var pendingMigrationsList = pendingMigrations.ToList();

            if (!pendingMigrationsList.Any())
            {
                _logger.LogInformation("No pending migrations found.");
                return;
            }

            _logger.LogInformation("Found {Count} pending migrations:", pendingMigrationsList.Count);
            foreach (var migration in pendingMigrationsList)
            {
                _logger.LogInformation("- {Migration}", migration);
            }

            await _context.Database.MigrateAsync();
            _logger.LogInformation("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database migration");
            throw;
        }
    }

    /// <summary>
    /// Check database connection
    /// </summary>
    public async Task<bool> CheckConnectionAsync()
    {
        try
        {
            _logger.LogInformation("Checking database connection...");
            var canConnect = await _context.Database.CanConnectAsync();

            if (canConnect)
            {
                _logger.LogInformation("Database connection successful.");
            }
            else
            {
                _logger.LogWarning("Cannot connect to database.");
            }

            return canConnect;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking database connection");
            return false;
        }
    }

    /// <summary>
    /// Get applied migrations
    /// </summary>
    public async Task<IEnumerable<string>> GetAppliedMigrationsAsync()
    {
        try
        {
            _logger.LogInformation("Getting applied migrations...");
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

            _logger.LogInformation("Found {Count} applied migrations.", appliedMigrations.Count());
            return appliedMigrations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applied migrations");
            throw;
        }
    }

    /// <summary>
    /// Get pending migrations
    /// </summary>
    public async Task<IEnumerable<string>> GetPendingMigrationsAsync()
    {
        try
        {
            _logger.LogInformation("Getting pending migrations...");
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

            _logger.LogInformation("Found {Count} pending migrations.", pendingMigrations.Count());
            return pendingMigrations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending migrations");
            throw;
        }
    }

    /// <summary>
    /// Create database if it doesn't exist
    /// </summary>
    public async Task EnsureDatabaseCreatedAsync()
    {
        try
        {
            _logger.LogInformation("Ensuring database is created...");
            var created = await _context.Database.EnsureCreatedAsync();

            if (created)
            {
                _logger.LogInformation("Database created successfully.");
            }
            else
            {
                _logger.LogInformation("Database already exists.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating database");
            throw;
        }
    }

    /// <summary>
    /// Drop database if it exists
    /// </summary>
    public async Task DropDatabaseAsync()
    {
        try
        {
            _logger.LogWarning("Dropping database...");
            var dropped = await _context.Database.EnsureDeletedAsync();

            if (dropped)
            {
                _logger.LogInformation("Database dropped successfully.");
            }
            else
            {
                _logger.LogInformation("Database did not exist.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dropping database");
            throw;
        }
    }

    /// <summary>
    /// Get database information
    /// </summary>
    public async Task ShowDatabaseInfoAsync()
    {
        try
        {
            _logger.LogInformation("=== Database Information ===");

            var connectionString = _context.Database.GetConnectionString();
            _logger.LogInformation("Connection String: {ConnectionString}", connectionString);

            var canConnect = await _context.Database.CanConnectAsync();
            _logger.LogInformation("Can Connect: {CanConnect}", canConnect);

            if (canConnect)
            {
                var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

                _logger.LogInformation("Applied Migrations ({Count}):", appliedMigrations.Count());
                foreach (var migration in appliedMigrations)
                {
                    _logger.LogInformation("  ✓ {Migration}", migration);
                }

                _logger.LogInformation("Pending Migrations ({Count}):", pendingMigrations.Count());
                foreach (var migration in pendingMigrations)
                {
                    _logger.LogInformation("  ⏳ {Migration}", migration);
                }
            }

            _logger.LogInformation("=== End Database Information ===");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database information");
            throw;
        }
    }
}
