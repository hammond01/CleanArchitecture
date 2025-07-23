using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderManagement.Migrator.Data;
using OrderManagement.Migrator.Services;

namespace OrderManagement.Migrator;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Order Management Database Migrator ===");
        Console.WriteLine();

        try
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            // Create logger
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            var logger = loggerFactory.CreateLogger<Program>();

            // Create DbContext
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<OrderManagementDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            using var context = new OrderManagementDbContext(optionsBuilder.Options);
            var migrationService = new MigrationService(context, configuration, loggerFactory.CreateLogger<MigrationService>());
            var seedingService = new DataSeedingService(context, loggerFactory.CreateLogger<DataSeedingService>());

            // Parse command line arguments
            var command = args.Length > 0 ? args[0].ToLower() : "help";

            logger.LogInformation("Starting migration tool with command: {Command}", command);

            switch (command)
            {
                case "migrate":
                case "m":
                    await RunMigrationAsync(migrationService, seedingService, configuration);
                    break;

                case "seed":
                case "s":
                    await RunSeedingAsync(seedingService);
                    break;

                case "info":
                case "i":
                    await ShowDatabaseInfoAsync(migrationService, seedingService);
                    break;

                case "drop":
                case "d":
                    await DropDatabaseAsync(migrationService);
                    break;

                case "reset":
                case "r":
                    await ResetDatabaseAsync(migrationService, seedingService, configuration);
                    break;

                case "clear":
                case "c":
                    await ClearDataAsync(seedingService);
                    break;

                case "help":
                case "h":
                case "?":
                default:
                    ShowHelp();
                    break;
            }

            logger.LogInformation("Migration tool completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Environment.Exit(1);
        }
    }



    private static async Task RunMigrationAsync(MigrationService migrationService, DataSeedingService seedingService, IConfiguration configuration)
    {
        Console.WriteLine("Running database migration...");

        await migrationService.CheckConnectionAsync();
        await migrationService.ApplyMigrationsAsync();

        var shouldSeed = configuration.GetValue<bool>("Migration:SeedData");
        if (shouldSeed)
        {
            Console.WriteLine("Seeding initial data...");
            await seedingService.SeedDataAsync();
        }

        Console.WriteLine("Migration completed successfully!");
    }

    private static async Task RunSeedingAsync(DataSeedingService seedingService)
    {
        Console.WriteLine("Seeding database with sample data...");
        await seedingService.SeedDataAsync();
        await seedingService.ShowDataStatisticsAsync();
        Console.WriteLine("Seeding completed successfully!");
    }

    private static async Task ShowDatabaseInfoAsync(MigrationService migrationService, DataSeedingService seedingService)
    {
        Console.WriteLine("Getting database information...");
        await migrationService.ShowDatabaseInfoAsync();
        await seedingService.ShowDataStatisticsAsync();
    }

    private static async Task DropDatabaseAsync(MigrationService migrationService)
    {
        Console.WriteLine("WARNING: This will permanently delete the database!");
        Console.Write("Are you sure you want to continue? (y/N): ");
        var confirmation = Console.ReadLine();

        if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
        {
            Console.WriteLine("Dropping database...");
            await migrationService.DropDatabaseAsync();
            Console.WriteLine("Database dropped successfully!");
        }
        else
        {
            Console.WriteLine("Operation cancelled.");
        }
    }

    private static async Task ResetDatabaseAsync(MigrationService migrationService, DataSeedingService seedingService, IConfiguration configuration)
    {
        Console.WriteLine("WARNING: This will drop and recreate the database!");
        Console.Write("Are you sure you want to continue? (y/N): ");
        var confirmation = Console.ReadLine();

        if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
        {
            Console.WriteLine("Resetting database...");
            await migrationService.DropDatabaseAsync();
            await migrationService.ApplyMigrationsAsync();

            var shouldSeed = configuration.GetValue<bool>("Migration:SeedData");
            if (shouldSeed)
            {
                Console.WriteLine("Seeding initial data...");
                await seedingService.SeedDataAsync();
            }

            Console.WriteLine("Database reset completed successfully!");
        }
        else
        {
            Console.WriteLine("Operation cancelled.");
        }
    }

    private static async Task ClearDataAsync(DataSeedingService seedingService)
    {
        Console.WriteLine("WARNING: This will delete all data from the database!");
        Console.Write("Are you sure you want to continue? (y/N): ");
        var confirmation = Console.ReadLine();

        if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
        {
            Console.WriteLine("Clearing data...");
            await seedingService.ClearDataAsync();
            Console.WriteLine("Data cleared successfully!");
        }
        else
        {
            Console.WriteLine("Operation cancelled.");
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Order Management Database Migrator");
        Console.WriteLine("Usage: dotnet run [command]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  migrate, m    Apply pending migrations and optionally seed data");
        Console.WriteLine("  seed, s       Seed database with sample data");
        Console.WriteLine("  info, i       Show database and data information");
        Console.WriteLine("  drop, d       Drop the database (WARNING: destructive)");
        Console.WriteLine("  reset, r      Drop and recreate database with migrations and seed data");
        Console.WriteLine("  clear, c      Clear all data from database (WARNING: destructive)");
        Console.WriteLine("  help, h, ?    Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run migrate    # Apply migrations and seed data");
        Console.WriteLine("  dotnet run info       # Show database information");
        Console.WriteLine("  dotnet run seed       # Seed sample data");
        Console.WriteLine("  dotnet run reset      # Reset database completely");
        Console.WriteLine();
        Console.WriteLine("Configuration:");
        Console.WriteLine("  Connection string: appsettings.json -> ConnectionStrings:DefaultConnection");
        Console.WriteLine("  Auto seed data: appsettings.json -> Migration:SeedData");
        Console.WriteLine("  Environment: Set ASPNETCORE_ENVIRONMENT variable");
    }
}
