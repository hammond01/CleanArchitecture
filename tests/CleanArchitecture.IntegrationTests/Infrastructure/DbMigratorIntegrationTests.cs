using Auditing.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence;
using DbMigrator;
using FluentAssertions;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace CleanArchitecture.IntegrationTests.Infrastructure;

/// <summary>
/// Database integration tests using PostgreSQL Testcontainers
/// Set USE_TESTCONTAINERS=true to enable these tests.
/// </summary>
public class DbMigratorIntegrationTests : IAsyncLifetime
{
    private readonly bool _useTestcontainers;
    private IContainer? _container;
    private string? _connectionString;

    public DbMigratorIntegrationTests()
    {
        _useTestcontainers = string.Equals(
            Environment.GetEnvironmentVariable("USE_TESTCONTAINERS"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (_useTestcontainers)
        {
            _container = new ContainerBuilder()
                .WithImage("postgres:16")
                .WithName($"cleanarchitecture-dbmigrator-tests-{Guid.NewGuid():N}")
                .WithEnvironment("POSTGRES_DB", "CleanArchitectureTests")
                .WithEnvironment("POSTGRES_USER", "postgres")
                .WithEnvironment("POSTGRES_PASSWORD", "postgres")
                .WithPortBinding(5432, true)
                .WithCleanUp(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
                .Build();
        }
    }

    public async Task InitializeAsync()
    {
        if (_useTestcontainers && _container != null)
        {
            await _container.StartAsync();
            _connectionString = BuildConnectionString(_container);
        }
    }

    public async Task DisposeAsync()
    {
        if (_useTestcontainers && _container != null)
        {
            await _container.DisposeAsync().AsTask();
        }
    }

    [Fact]
    public async Task DbMigrator_Runs_All_Module_Migrations()
    {
        if (!_useTestcontainers || _container == null)
        {
            return;
        }

        var connectionString = _connectionString!;
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddConsole());
        services.Configure<MigrationSettings>(options =>
        {
            options.SeedData = false;
            options.CreateDatabaseIfNotExists = true;
            options.TimeoutSeconds = 300;
        });

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsHistoryTable("__EFMigrationsHistory", "identity")));

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsHistoryTable("__EFMigrationsHistory", "catalog")));

        services.AddDbContext<AuditingDbContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsHistoryTable("__EFMigrationsHistory", "auditing")));

        services.AddScoped<DbMigrationService>();

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var migrator = scope.ServiceProvider.GetRequiredService<DbMigrationService>();
        await migrator.MigrateAsync();

        var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var catalogDb = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var auditingDb = scope.ServiceProvider.GetRequiredService<AuditingDbContext>();

        (await identityDb.Database.CanConnectAsync()).Should().BeTrue();
        (await catalogDb.Database.CanConnectAsync()).Should().BeTrue();
        (await auditingDb.Database.CanConnectAsync()).Should().BeTrue();

        (await identityDb.Database.GetPendingMigrationsAsync()).Should().BeEmpty();
        (await catalogDb.Database.GetPendingMigrationsAsync()).Should().BeEmpty();
        (await auditingDb.Database.GetPendingMigrationsAsync()).Should().BeEmpty();
    }

    private static string BuildConnectionString(IContainer container)
    {
        var port = container.GetMappedPublicPort(5432);
        return $"Host=localhost;Port={port};Database=CleanArchitectureTests;Username=postgres;Password=postgres";
    }
}
