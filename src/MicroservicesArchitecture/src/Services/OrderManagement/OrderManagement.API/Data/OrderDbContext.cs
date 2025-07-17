using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Models;

namespace OrderManagement.API.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingAddress).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.OrderDate);
            entity.HasIndex(e => e.Status);
        });

        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Order)
                  .WithMany(e => e.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProductId);
        });

        // Configure Customer entity
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.ZipCode).HasMaxLength(10);
            entity.Property(e => e.Country).HasMaxLength(100);
            
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Phone);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                CustomerId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1-555-0123",
                Address = "123 Main St",
                City = "New York",
                State = "NY",
                ZipCode = "10001",
                Country = "USA",
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                CustomerId = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "+1-555-0456",
                Address = "456 Oak Ave",
                City = "Los Angeles",
                State = "CA",
                ZipCode = "90001",
                Country = "USA",
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                CustomerId = 3,
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@example.com",
                Phone = "+1-555-0789",
                Address = "789 Pine Rd",
                City = "Chicago",
                State = "IL",
                ZipCode = "60601",
                Country = "USA",
                CreatedAt = DateTime.UtcNow
            }
        );

        // Seed Orders
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                OrderId = 1,
                OrderNumber = "ORD-2025-001",
                CustomerId = 1,
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                OrderDate = DateTime.UtcNow.AddDays(-7),
                Status = OrderStatus.Delivered,
                TotalAmount = 299.99m,
                ShippingAddress = "123 Main St, New York, NY 10001",
                Notes = "Expedited shipping requested",
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new Order
            {
                OrderId = 2,
                OrderNumber = "ORD-2025-002",
                CustomerId = 2,
                CustomerName = "Jane Smith",
                CustomerEmail = "jane.smith@example.com",
                OrderDate = DateTime.UtcNow.AddDays(-3),
                Status = OrderStatus.Shipped,
                TotalAmount = 459.50m,
                ShippingAddress = "456 Oak Ave, Los Angeles, CA 90001",
                Notes = "Standard shipping",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Order
            {
                OrderId = 3,
                OrderNumber = "ORD-2025-003",
                CustomerId = 3,
                CustomerName = "Bob Johnson",
                CustomerEmail = "bob.johnson@example.com",
                OrderDate = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Processing,
                TotalAmount = 129.99m,
                ShippingAddress = "789 Pine Rd, Chicago, IL 60601",
                Notes = "Gift wrapping requested",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        );

        // Seed OrderItems
        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem
            {
                OrderItemId = 1,
                OrderId = 1,
                ProductId = 1,
                ProductName = "Laptop Computer",
                Quantity = 1,
                UnitPrice = 299.99m,
                TotalPrice = 299.99m
            },
            new OrderItem
            {
                OrderItemId = 2,
                OrderId = 2,
                ProductId = 2,
                ProductName = "Smartphone",
                Quantity = 1,
                UnitPrice = 399.99m,
                TotalPrice = 399.99m
            },
            new OrderItem
            {
                OrderItemId = 3,
                OrderId = 2,
                ProductId = 3,
                ProductName = "Wireless Headphones",
                Quantity = 1,
                UnitPrice = 59.51m,
                TotalPrice = 59.51m
            },
            new OrderItem
            {
                OrderItemId = 4,
                OrderId = 3,
                ProductId = 4,
                ProductName = "Tablet",
                Quantity = 1,
                UnitPrice = 129.99m,
                TotalPrice = 129.99m
            }
        );
    }
}
