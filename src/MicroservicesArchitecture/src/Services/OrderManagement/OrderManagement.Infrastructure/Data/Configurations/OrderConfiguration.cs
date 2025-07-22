using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Order
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Table configuration
        builder.ToTable("Orders");

        // Primary key
        builder.HasKey(o => o.Id);

        // Properties
        builder.Property(o => o.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.OrderNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.CustomerId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt);

        // Indexes
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique()
            .HasDatabaseName("IX_Orders_OrderNumber");

        builder.HasIndex(o => o.CustomerId)
            .HasDatabaseName("IX_Orders_CustomerId");

        builder.HasIndex(o => o.OrderDate)
            .HasDatabaseName("IX_Orders_OrderDate");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("IX_Orders_Status");

        // Relationships
        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        // Value objects
        builder.OwnsOne(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(o => o.CustomerInfo, customerInfo =>
        {
            customerInfo.Property(c => c.Name)
                .HasColumnName("CustomerName")
                .HasMaxLength(100)
                .IsRequired();

            customerInfo.Property(c => c.Email)
                .HasColumnName("CustomerEmail")
                .HasMaxLength(200)
                .IsRequired();

            customerInfo.Property(c => c.Phone)
                .HasColumnName("CustomerPhone")
                .HasMaxLength(20);
        });

        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("ShippingStreet")
                .HasMaxLength(200);

            address.Property(a => a.City)
                .HasColumnName("ShippingCity")
                .HasMaxLength(100);

            address.Property(a => a.State)
                .HasColumnName("ShippingState")
                .HasMaxLength(100);

            address.Property(a => a.ZipCode)
                .HasColumnName("ShippingZipCode")
                .HasMaxLength(20);

            address.Property(a => a.Country)
                .HasColumnName("ShippingCountry")
                .HasMaxLength(100);
        });
    }
}
