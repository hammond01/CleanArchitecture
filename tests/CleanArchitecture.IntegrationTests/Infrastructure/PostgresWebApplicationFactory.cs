using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Catalog.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence;
using Auditing.Infrastructure.Persistence;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace CleanArchitecture.IntegrationTests.Infrastructure;

/// <summary>
/// WebApplicationFactory using PostgreSQL Testcontainers for integration tests
/// </summary>
public class PostgresWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private IContainer? _container;
    private bool _useTestcontainers;
    private string? _connectionString;

    public PostgresWebApplicationFactory()
    {
        _useTestcontainers = string.Equals(
            Environment.GetEnvironmentVariable("USE_TESTCONTAINERS"),
            "true",
            StringComparison.OrdinalIgnoreCase);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (_useTestcontainers && TryEnsureContainer())
        {
            _connectionString = BuildConnectionString(_container!);

            builder.ConfigureAppConfiguration((context, config) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _connectionString,
                    ["EmailSettings:UseFakeEmail"] = "true"
                };

                config.AddInMemoryCollection(settings);
            });
        }

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<CatalogDbContext>));
            services.RemoveAll(typeof(DbContextOptions<IdentityDbContext>));
            services.RemoveAll(typeof(DbContextOptions<AuditingDbContext>));

            if (_useTestcontainers && _connectionString != null)
            {
                services.AddDbContext<CatalogDbContext>(options =>
                    options.UseNpgsql(_connectionString,
                        b => b.MigrationsHistoryTable("__ef_migrations_history", "catalog"))
                        .UseSnakeCaseNamingConvention());

                services.AddDbContext<IdentityDbContext>(options =>
                    options.UseNpgsql(_connectionString,
                        b => b.MigrationsHistoryTable("__ef_migrations_history", "identity"))
                        .UseSnakeCaseNamingConvention());

                services.AddDbContext<AuditingDbContext>(options =>
                    options.UseNpgsql(_connectionString,
                        b => b.MigrationsHistoryTable("__ef_migrations_history", "auditing"))
                        .UseSnakeCaseNamingConvention());
            }
            else
            {
                services.AddDbContext<CatalogDbContext>(options =>
                    options.UseInMemoryDatabase("TestCatalogDb"));

                services.AddDbContext<IdentityDbContext>(options =>
                    options.UseInMemoryDatabase("TestIdentityDb"));

                services.AddDbContext<AuditingDbContext>(options =>
                    options.UseInMemoryDatabase("TestAuditingDb"));
            }

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var catalogDb = scopedServices.GetRequiredService<CatalogDbContext>();
            var identityDb = scopedServices.GetRequiredService<IdentityDbContext>();
            var auditingDb = scopedServices.GetRequiredService<AuditingDbContext>();

            if (_useTestcontainers && _connectionString != null)
            {
                catalogDb.Database.Migrate();
                identityDb.Database.Migrate();
                auditingDb.Database.Migrate();
            }
            else
            {
                catalogDb.Database.EnsureCreated();
                identityDb.Database.EnsureCreated();
                auditingDb.Database.EnsureCreated();
            }
        });

        builder.UseEnvironment("Test");
    }

    public Task InitializeAsync()
    {
        if (!_useTestcontainers || !TryEnsureContainer() || _container == null)
        {
            return Task.CompletedTask;
        }

        return _container.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        if (!_useTestcontainers || _container == null)
        {
            return Task.CompletedTask;
        }

        return _container.DisposeAsync().AsTask();
    }

    private bool TryEnsureContainer()
    {
        if (_container != null)
        {
            return true;
        }

        try
        {
            _container = new ContainerBuilder()
                .WithImage("postgres:16")
                .WithName($"cleanarchitecture-api-tests-{Guid.NewGuid():N}")
                .WithEnvironment("POSTGRES_DB", "CleanArchitectureTests")
                .WithEnvironment("POSTGRES_USER", "postgres")
                .WithEnvironment("POSTGRES_PASSWORD", "postgres")
                .WithPortBinding(5432, true)
                .WithCleanUp(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
                .Build();
            return true;
        }
        catch (ArgumentException)
        {
            _useTestcontainers = false;
            return false;
        }
    }

    private static string BuildConnectionString(IContainer container)
    {
        var port = container.GetMappedPublicPort(5432);
        return $"Host=localhost;Port={port};Database=CleanArchitectureTests;Username=postgres;Password=postgres";
    }
}
