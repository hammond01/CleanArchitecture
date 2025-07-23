using Microsoft.EntityFrameworkCore;

namespace OrderManagement.Migrator;

/// <summary>
/// Basic database migrator without configuration dependencies
/// </summary>
public class BasicMigrator
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Order Management Database Migrator ===");
        Console.WriteLine();

        try
        {
            // Parse command line arguments
            var command = args.Length > 0 ? args[0].ToLower() : "help";

            // Use hardcoded connection string for now
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=OrderManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true";

            // Allow override via environment variable
            var envConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnectionString))
            {
                connectionString = envConnectionString;
            }

            var optionsBuilder = new DbContextOptionsBuilder<SimpleOrderDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            using var context = new SimpleOrderDbContext(optionsBuilder.Options);

            Console.WriteLine($"Starting migration tool with command: {command}");
            Console.WriteLine($"Connection: {connectionString}");
            Console.WriteLine();

            switch (command)
            {
                case "migrate":
                case "m":
                    await RunMigrationAsync(context);
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
                    await ResetDatabaseAsync(context);
                    break;

                case "seed":
                case "s":
                    await SeedDataAsync(context);
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

    private static async Task RunMigrationAsync(SimpleOrderDbContext context)
    {
        Console.WriteLine("Running database migration...");

        await CheckConnectionAsync(context);
        await ApplyMigrationsAsync(context);
        await SeedDataAsync(context);

        Console.WriteLine("Migration completed successfully!");
    }

    private static async Task ShowDatabaseInfoAsync(SimpleOrderDbContext context)
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

            // Show data statistics
            try
            {
                var orderCount = await context.Orders.CountAsync();
                var orderItemCount = await context.OrderItems.CountAsync();
                Console.WriteLine($"Total Orders: {orderCount}");
                Console.WriteLine($"Total Order Items: {orderItemCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not get data statistics: {ex.Message}");
            }
        }

        Console.WriteLine("=== End Database Information ===");
    }

    private static async Task DropDatabaseAsync(SimpleOrderDbContext context)
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

    private static async Task ResetDatabaseAsync(SimpleOrderDbContext context)
    {
        Console.WriteLine("WARNING: This will drop and recreate the database!");
        Console.Write("Are you sure you want to continue? (y/N): ");
        var confirmation = Console.ReadLine();

        if (confirmation?.ToLower() == "y" || confirmation?.ToLower() == "yes")
        {
            Console.WriteLine("Resetting database...");
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
            await SeedDataAsync(context);
            Console.WriteLine("Database reset completed successfully!");
        }
        else
        {
            Console.WriteLine("Operation cancelled.");
        }
    }

    private static async Task CheckConnectionAsync(SimpleOrderDbContext context)
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

    private static async Task ApplyMigrationsAsync(SimpleOrderDbContext context)
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

    private static async Task SeedDataAsync(SimpleOrderDbContext context)
    {
        Console.WriteLine("Seeding sample data...");

        // Check if data already exists
        var existingOrdersCount = await context.Orders.CountAsync();
        if (existingOrdersCount > 0)
        {
            Console.WriteLine("Data already exists. Skipping seeding.");
            return;
        }

        // Create sample orders
        var sampleOrders = new List<SimpleOrder>
        {
            new SimpleOrder
            {
                Id = Guid.NewGuid().ToString(),
                OrderNumber = "ORD-001",
                CustomerId = "CUST-001",
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                Status = "Pending",
                TotalAmount = 1059.97m,
                Currency = "USD",
                Notes = "First sample order",
                CreatedAt = DateTime.UtcNow
            },
            new SimpleOrder
            {
                Id = Guid.NewGuid().ToString(),
                OrderNumber = "ORD-002",
                CustomerId = "CUST-002",
                CustomerName = "Jane Smith",
                CustomerEmail = "jane.smith@example.com",
                Status = "Confirmed",
                TotalAmount = 719.98m,
                Currency = "USD",
                Notes = "Second sample order",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new SimpleOrder
            {
                Id = Guid.NewGuid().ToString(),
                OrderNumber = "ORD-003",
                CustomerId = "CUST-003",
                CustomerName = "Bob Johnson",
                CustomerEmail = "bob.johnson@example.com",
                Status = "Processing",
                TotalAmount = 449.98m,
                Currency = "USD",
                Notes = "Third sample order",
                CreatedAt = DateTime.UtcNow.AddHours(-4)
            }
        };

        await context.Orders.AddRangeAsync(sampleOrders);
        await context.SaveChangesAsync();

        Console.WriteLine($"Added {sampleOrders.Count} sample orders");
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Order Management Database Migrator");
        Console.WriteLine("Usage: dotnet run [command]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  migrate, m    Apply pending migrations and seed data");
        Console.WriteLine("  info, i       Show database and data information");
        Console.WriteLine("  drop, d       Drop the database (WARNING: destructive)");
        Console.WriteLine("  reset, r      Drop and recreate database with migrations and seed data");
        Console.WriteLine("  seed, s       Seed sample data only");
        Console.WriteLine("  help, h, ?    Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run migrate    # Apply migrations and seed data");
        Console.WriteLine("  dotnet run info       # Show database information");
        Console.WriteLine("  dotnet run reset      # Reset database completely");
        Console.WriteLine();
        Console.WriteLine("Configuration:");
        Console.WriteLine("  Set CONNECTION_STRING environment variable to override default connection");
        Console.WriteLine("  Default: Server=(localdb)\\mssqllocaldb;Database=OrderManagementDb;Trusted_Connection=true");
    }
}

/// <summary>
/// Simple DbContext for migrations only
/// </summary>
public class SimpleOrderDbContext : DbContext
{
    public SimpleOrderDbContext(DbContextOptions<SimpleOrderDbContext> options) : base(options)
    {
    }

    public DbSet<SimpleOrder> Orders { get; set; } = null!;
    public DbSet<SimpleOrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Simple table definitions for migration purposes
        modelBuilder.Entity<SimpleOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.OrderNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CustomerId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CustomerName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CustomerEmail).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
        });

        modelBuilder.Entity<SimpleOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.OrderId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProductId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(3);

            entity.HasOne<SimpleOrder>()
                .WithMany()
                .HasForeignKey(e => e.OrderId)
                .HasPrincipalKey(e => e.Id);
        });
    }
}

public class SimpleOrder
{
    public string Id { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SimpleOrderItem
{
    public string Id { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Currency { get; set; } = "USD";
}
