using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Migrator.Data;

/// <summary>
/// DbContext for Order Management - Migrator version
/// </summary>
public class OrderManagementDbContext : DbContext
{
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Number);
            entity.Property(e => e.Number).HasMaxLength(50);
            entity.Property(e => e.OrderNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CustomerId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            // Configure Status enum
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Configure CustomerInfo as owned entity
            entity.OwnsOne(e => e.CustomerInfo, customerInfo =>
            {
                customerInfo.Property(ci => ci.Name).HasMaxLength(100).IsRequired();
                customerInfo.Property(ci => ci.Email).HasMaxLength(100);
                customerInfo.Property(ci => ci.Phone).HasMaxLength(20);
            });

            // Configure ShippingAddress as owned entity
            entity.OwnsOne(e => e.ShippingAddress, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.State).HasMaxLength(50);
                address.Property(a => a.PostalCode).HasMaxLength(20);
                address.Property(a => a.Country).HasMaxLength(100);
            });

            // Configure TotalAmount as owned entity
            entity.OwnsOne(e => e.TotalAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasMaxLength(3);
            });

            // Configure relationship with OrderItems
            entity.HasMany<OrderItem>()
                .WithOne()
                .HasForeignKey("OrderNumber")
                .HasPrincipalKey(e => e.OrderNumber);
        });

        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Number);
            entity.Property(e => e.Number).HasMaxLength(50);
            entity.Property(e => e.ProductId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property("OrderNumber").HasMaxLength(50).IsRequired();

            // Configure UnitPrice as owned entity
            entity.OwnsOne(e => e.UnitPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasMaxLength(3);
            });

            // Configure TotalPrice as owned entity
            entity.OwnsOne(e => e.TotalPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasMaxLength(3);
            });
        });
    }
}
