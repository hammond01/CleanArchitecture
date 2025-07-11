using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Data.Configurations;

/// <summary>
/// Product entity configuration
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.ProductId);
        builder.Property(p => p.ProductId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.ProductName)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(p => p.SupplierId)
            .HasMaxLength(50);

        builder.Property(p => p.CategoryId)
            .HasMaxLength(50);

        builder.Property(p => p.QuantityPerUnit)
            .HasMaxLength(20);

        builder.Property(p => p.UnitPrice)
            .HasColumnType("money");

        builder.Property(p => p.UnitsInStock);
        builder.Property(p => p.UnitsOnOrder);
        builder.Property(p => p.ReorderLevel);
        builder.Property(p => p.Discontinued);

        // Indexes
        builder.HasIndex(p => p.ProductName).HasDatabaseName("IX_Products_ProductName");
        builder.HasIndex(p => p.CategoryId).HasDatabaseName("IX_Products_CategoryId");
        builder.HasIndex(p => p.SupplierId).HasDatabaseName("IX_Products_SupplierId");

        // Seed data
        builder.HasData(
            new
            {
                ProductId = "01JH179GGZ7FAHZ0DNFYNZ20AA",
                ProductName = "Laptop Dell XPS 13",
                SupplierId = "01JH179GGZ7FAHZ0DNFYNZ18YX",
                CategoryId = "01JH179GGZ7FAHZ0DNFYNZ21BB",
                QuantityPerUnit = "1 unit",
                UnitPrice = 1299.99m,
                UnitsInStock = (short)50,
                UnitsOnOrder = (short)0,
                ReorderLevel = (short)10,
                Discontinued = false,
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                ProductId = "01JH179GGZ7FAHZ0DNFYNZ22CC",
                ProductName = "iPhone 15 Pro",
                SupplierId = "01JH179GGZ7FAHZ0DNFYNZ19FG",
                CategoryId = "01JH179GGZ7FAHZ0DNFYNZ23DD",
                QuantityPerUnit = "1 unit",
                UnitPrice = 999.99m,
                UnitsInStock = (short)30,
                UnitsOnOrder = (short)20,
                ReorderLevel = (short)5,
                Discontinued = false,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
