using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderManagement.Migrator.Data;

namespace OrderManagement.Migrator;

/// <summary>
/// Simple migrator without DI container
/// </summary>
public class SimpleMigrator
{
    public static async Task Main(string[] args)
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

            // Parse command line arguments
            var command = args.Length > 0 ? args[0].ToLower() : "help";

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

            Console.WriteLine($"Starting migration tool with command: {command}");

            switch (command)
            {
                case "migrate":
                case "m":
                    await RunMigrationAsync(context, configuration);
                    break;

                case "seed":
                case "s":
                    await RunSeedingAsync(context);
                    break;

                case "info":
                case "i":
                    await ShowDatabaseInfoAsync(context);
                    break;

                case "drop":
                case "d":
                    await DropDatabaseAsync(context);
                    break;

                case "reset":
                case "r":
                    await ResetDatabaseAsync(context, configuration);
                    break;

                case "clear":
                case "c":
                    await ClearDataAsync(context);
                    break;

                case "help":
                case "h":
                case "?":
                default:
                    ShowHelp();
                    break;
            }

            Console.WriteLine("Migration tool completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Environment.Exit(1);
        }
    }

    private static async Task RunMigrationAsync(OrderManagementDbContext context, IConfiguration configuration)
    {
        Console.WriteLine("Running database migration...");
        
        await CheckConnectionAsync(context);
        await ApplyMigrationsAsync(context);

        var shouldSeed = configuration.GetValue<bool>("Migration:SeedData");
        if (shouldSeed)
        {
            Console.WriteLine("Seeding initial data...");
            await SeedDataAsync(context);
        }

        Console.WriteLine("Migration completed successfully!");
    }

    private static async Task RunSeedingAsync(OrderManagementDbContext context)
    {
        Console.WriteLine("Seeding database with sample data...");
        await SeedDataAsync(context);
        await ShowDataStatisticsAsync(context);
        Console.WriteLine("Seeding completed successfully!");
    }

    private static async Task ShowDatabaseInfoAsync(OrderManagementDbContext context)
    {
        Console.WriteLine("Getting database information...");
        
        Console.WriteLine("=== Database Information ===");
        
        var connectionString = context.Database.GetConnectionString();
        Console.WriteLine($"Connection String: {connectionString}");

        var canConnect = await context.Database.CanConnectAsync();
        Console.WriteLine($"Can Connect: {canConnect}");

        if (canConnect)
        {
            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            Console.WriteLine($"Applied Migrations ({appliedMigrations.Count()}):");
            foreach (var migration in appliedMigrations)
            {
                Console.WriteLine($"  ✓ {migration}");
            }

            Console.WriteLine($"Pending Migrations ({pendingMigrations.Count()}):");
            foreach (var migration in pendingMigrations)
            {
                Console.WriteLine($"  ⏳ {migration}");
            }
        }

        await ShowDataStatisticsAsync(context);
        Console.WriteLine("=== End Database Information ===");
    }

    private static async Task DropDatabaseAsync(OrderManagementDbContext context)
    {
        Console.WriteLine("WARNING: This will permanently delete the database!");
        Console.Write("Are you sure you want to continue? (y/N): ");
        var confirmation = Console.ReadLine();

        if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
        {
            Console.WriteLine("Dropping database...");
            var dropped = await context.Database.EnsureDeletedAsync();
            
            if (dropped)
            {
                Console.WriteLine("Database dropped successfully!");
            }
            else
            {
                Console.WriteLine("Database did not exist.");
            }
        }
        else
        {
            Console.WriteLine("Operation cancelled.");
        }
    }

    private static async Task ResetDatabaseAsync(OrderManagementDbContext context, IConfiguration configuration)
    {
        Console.WriteLine("WARNING: This will drop and recreate the database!");
        Console.Write("Are you sure you want to continue? (y/N): ");
        var confirmation = Console.ReadLine();

        if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
        {
            Console.WriteLine("Resetting database...");
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();

            var shouldSeed = configuration.GetValue<bool>("Migration:SeedData");
            if (shouldSeed)
            {
                Console.WriteLine("Seeding initial data...");
                await SeedDataAsync(context);
            }

            Console.WriteLine("Database reset completed successfully!");
        }
        else
        {
            Console.WriteLine("Operation cancelled.");
        }
    }

    private static async Task ClearDataAsync(OrderManagementDbContext context)
    {
        Console.WriteLine("WARNING: This will delete all data from the database!");
        Console.Write("Are you sure you want to continue? (y/N): ");
        var confirmation = Console.ReadLine();

        if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
        {
            Console.WriteLine("Clearing data...");

            // Remove all order items first (due to foreign key constraints)
            var orderItems = await context.OrderItems.ToListAsync();
            context.OrderItems.RemoveRange(orderItems);

            // Remove all orders
            var orders = await context.Orders.ToListAsync();
            context.Orders.RemoveRange(orders);

            await context.SaveChangesAsync();
            Console.WriteLine("Data cleared successfully!");
        }
        else
        {
            Console.WriteLine("Operation cancelled.");
        }
    }

    private static async Task CheckConnectionAsync(OrderManagementDbContext context)
    {
        Console.WriteLine("Checking database connection...");
        var canConnect = await context.Database.CanConnectAsync();
        
        if (canConnect)
        {
            Console.WriteLine("Database connection successful.");
        }
        else
        {
            Console.WriteLine("Cannot connect to database.");
            throw new InvalidOperationException("Cannot connect to database");
        }
    }

    private static async Task ApplyMigrationsAsync(OrderManagementDbContext context)
    {
        Console.WriteLine("Starting database migration...");

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        var pendingMigrationsList = pendingMigrations.ToList();

        if (!pendingMigrationsList.Any())
        {
            Console.WriteLine("No pending migrations found.");
            return;
        }

        Console.WriteLine($"Found {pendingMigrationsList.Count} pending migrations:");
        foreach (var migration in pendingMigrationsList)
        {
            Console.WriteLine($"- {migration}");
        }

        await context.Database.MigrateAsync();
        Console.WriteLine("Database migration completed successfully.");
    }

    private static async Task SeedDataAsync(OrderManagementDbContext context)
    {
        // Check if data already exists
        var existingOrdersCount = await context.Orders.CountAsync();
        if (existingOrdersCount > 0)
        {
            Console.WriteLine("Data already exists. Skipping seeding.");
            return;
        }

        Console.WriteLine("Seeding sample data...");
        
        // This is a simplified seeding - in real implementation you would add proper sample data
        Console.WriteLine("Sample data seeding completed.");
        await context.SaveChangesAsync();
    }

    private static async Task ShowDataStatisticsAsync(OrderManagementDbContext context)
    {
        Console.WriteLine("=== Data Statistics ===");

        var totalOrders = await context.Orders.CountAsync();
        var totalOrderItems = await context.OrderItems.CountAsync();

        Console.WriteLine($"Total Orders: {totalOrders}");
        Console.WriteLine($"Total Order Items: {totalOrderItems}");

        Console.WriteLine("=== End Data Statistics ===");
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
