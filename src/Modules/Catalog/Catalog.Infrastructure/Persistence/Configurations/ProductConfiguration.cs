using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "catalog");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(p => p.CategoryId)
            .IsRequired()
            .HasColumnName("CategoryID");

        builder.Property(p => p.QuantityPerUnit)
            .HasMaxLength(20);

        builder.Property(p => p.UnitPrice)
            .HasColumnType("money");

        builder.Property(p => p.Discontinued)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.ProductName).HasDatabaseName("ProductName");
        builder.HasIndex(p => p.CategoryId).HasDatabaseName("CategoryID");

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
