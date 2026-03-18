using Auditing.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence;
using DbMigrator;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("╔═══════════════════════════════════════════════╗");
    Log.Information("║   🛠️  Clean Architecture DB Migrator         ║");
    Log.Information("║   Database Migration & Seeding Tool          ║");
    Log.Information("╚═══════════════════════════════════════════════╝");
    Log.Information("");

    var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
            config.AddEnvironmentVariables();
            config.AddCommandLine(args);
        })
        .UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        })
        .ConfigureServices((context, services) =>
        {
            var configuration = context.Configuration;
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

            // Register DbContexts
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(connectionString,
                    b => b.MigrationsHistoryTable("__EFMigrationsHistory", "identity")));

            services.AddDbContext<CatalogDbContext>(options =>
                options.UseNpgsql(connectionString,
                    b => b.MigrationsHistoryTable("__EFMigrationsHistory", "catalog")));

            services.AddDbContext<AuditingDbContext>(options =>
                options.UseNpgsql(connectionString,
                    b => b.MigrationsHistoryTable("__EFMigrationsHistory", "auditing")));

            // Register Migration Service
            services.Configure<MigrationSettings>(configuration.GetSection("MigrationSettings"));
            services.AddScoped<DbMigrationService>();
        })
        .Build();

    // Run migrations
    using (var scope = host.Services.CreateScope())
    {
        var migrationService = scope.ServiceProvider.GetRequiredService<DbMigrationService>();
        await migrationService.MigrateAsync();
    }

    Log.Information("");
    Log.Information("🎉 Migration process completed successfully!");
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "💥 Migration process terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

