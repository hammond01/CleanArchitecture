using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;
using OrderManagement.Migrator.Data;

namespace OrderManagement.Migrator.Services;

/// <summary>
/// Service for seeding initial data
/// </summary>
public class DataSeedingService
{
    private readonly OrderManagementDbContext _context;
    private readonly ILogger<DataSeedingService> _logger;

    public DataSeedingService(OrderManagementDbContext context, ILogger<DataSeedingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seed initial data
    /// </summary>
    public async Task SeedDataAsync()
    {
        try
        {
            _logger.LogInformation("Starting data seeding...");

            // Check if data already exists
            var existingOrdersCount = await _context.Orders.CountAsync();
            if (existingOrdersCount > 0)
            {
                _logger.LogInformation("Data already exists. Skipping seeding.");
                return;
            }

            await SeedSampleOrdersAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during data seeding");
            throw;
        }
    }

    /// <summary>
    /// Seed sample orders
    /// </summary>
    private async Task SeedSampleOrdersAsync()
    {
        _logger.LogInformation("Seeding sample orders...");

        var sampleOrders = new List<Order>();

        // Sample Order 1 - Pending
        var customer1 = CustomerInfo.Create("John Doe", "john.doe@example.com", "+1-555-0123");
        var address1 = Address.Create("123 Main St", "New York", "NY", "10001", "USA");
        var order1 = Order.Create("CUST001", customer1, address1, "First sample order");

        order1.AddOrderItem("PROD001", "Laptop Computer", 1, Money.Create(999.99m, "USD"));
        order1.AddOrderItem("PROD002", "Wireless Mouse", 2, Money.Create(29.99m, "USD"));
        sampleOrders.Add(order1);

        // Sample Order 2 - Confirmed
        var customer2 = CustomerInfo.Create("Jane Smith", "jane.smith@example.com", "+1-555-0456");
        var address2 = Address.Create("456 Oak Ave", "Los Angeles", "CA", "90210", "USA");
        var order2 = Order.Create("CUST002", customer2, address2, "Second sample order");

        order2.AddOrderItem("PROD003", "Smartphone", 1, Money.Create(699.99m, "USD"));
        order2.AddOrderItem("PROD004", "Phone Case", 1, Money.Create(19.99m, "USD"));
        order2.ConfirmOrder();
        sampleOrders.Add(order2);

        // Sample Order 3 - Processing
        var customer3 = CustomerInfo.Create("Bob Johnson", "bob.johnson@example.com", "+1-555-0789");
        var address3 = Address.Create("789 Pine St", "Chicago", "IL", "60601", "USA");
        var order3 = Order.Create("CUST003", customer3, address3, "Third sample order");

        order3.AddOrderItem("PROD005", "Tablet", 1, Money.Create(399.99m, "USD"));
        order3.AddOrderItem("PROD006", "Tablet Stand", 1, Money.Create(49.99m, "USD"));
        order3.ConfirmOrder();
        order3.StartProcessing();
        sampleOrders.Add(order3);

        // Sample Order 4 - Shipped
        var customer4 = CustomerInfo.Create("Alice Brown", "alice.brown@example.com", "+1-555-0321");
        var address4 = Address.Create("321 Elm St", "Houston", "TX", "77001", "USA");
        var order4 = Order.Create("CUST004", customer4, address4, "Fourth sample order");

        order4.AddOrderItem("PROD007", "Headphones", 1, Money.Create(149.99m, "USD"));
        order4.AddOrderItem("PROD008", "USB Cable", 2, Money.Create(9.99m, "USD"));
        order4.ConfirmOrder();
        order4.StartProcessing();
        order4.Ship("TRACK123456789");
        sampleOrders.Add(order4);

        // Sample Order 5 - Delivered
        var customer5 = CustomerInfo.Create("Charlie Wilson", "charlie.wilson@example.com", "+1-555-0654");
        var address5 = Address.Create("654 Maple Dr", "Phoenix", "AZ", "85001", "USA");
        var order5 = Order.Create("CUST005", customer5, address5, "Fifth sample order");

        order5.AddOrderItem("PROD009", "Keyboard", 1, Money.Create(79.99m, "USD"));
        order5.AddOrderItem("PROD010", "Monitor", 1, Money.Create(299.99m, "USD"));
        order5.ConfirmOrder();
        order5.StartProcessing();
        order5.Ship("TRACK987654321");
        order5.Deliver();
        sampleOrders.Add(order5);

        // Sample Order 6 - Cancelled
        var customer6 = CustomerInfo.Create("Diana Davis", "diana.davis@example.com", "+1-555-0987");
        var address6 = Address.Create("987 Cedar Ln", "Philadelphia", "PA", "19101", "USA");
        var order6 = Order.Create("CUST006", customer6, address6, "Sixth sample order - cancelled");

        order6.AddOrderItem("PROD011", "Webcam", 1, Money.Create(89.99m, "USD"));
        order6.AddOrderItem("PROD012", "Microphone", 1, Money.Create(59.99m, "USD"));
        order6.Cancel("Customer requested cancellation");
        sampleOrders.Add(order6);

        // Add orders to context
        await _context.Orders.AddRangeAsync(sampleOrders);

        _logger.LogInformation("Added {Count} sample orders", sampleOrders.Count);
    }

    /// <summary>
    /// Clear all data
    /// </summary>
    public async Task ClearDataAsync()
    {
        try
        {
            _logger.LogWarning("Clearing all data...");

            // Remove all order items first (due to foreign key constraints)
            var orderItems = await _context.OrderItems.ToListAsync();
            _context.OrderItems.RemoveRange(orderItems);

            // Remove all orders
            var orders = await _context.Orders.ToListAsync();
            _context.Orders.RemoveRange(orders);

            await _context.SaveChangesAsync();
            _logger.LogInformation("All data cleared successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while clearing data");
            throw;
        }
    }

    /// <summary>
    /// Show data statistics
    /// </summary>
    public async Task ShowDataStatisticsAsync()
    {
        try
        {
            _logger.LogInformation("=== Data Statistics ===");

            var totalOrders = await _context.Orders.CountAsync();
            var totalOrderItems = await _context.OrderItems.CountAsync();

            _logger.LogInformation("Total Orders: {TotalOrders}", totalOrders);
            _logger.LogInformation("Total Order Items: {TotalOrderItems}", totalOrderItems);

            if (totalOrders > 0)
            {
                var ordersByStatus = await _context.Orders
                    .GroupBy(o => o.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                _logger.LogInformation("Orders by Status:");
                foreach (var statusGroup in ordersByStatus)
                {
                    _logger.LogInformation("  {Status}: {Count}", statusGroup.Status, statusGroup.Count);
                }

                var totalRevenue = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered)
                    .SumAsync(o => o.TotalAmount.Amount);

                _logger.LogInformation("Total Revenue (Delivered Orders): ${TotalRevenue:F2}", totalRevenue);
            }

            _logger.LogInformation("=== End Data Statistics ===");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data statistics");
            throw;
        }
    }
}
