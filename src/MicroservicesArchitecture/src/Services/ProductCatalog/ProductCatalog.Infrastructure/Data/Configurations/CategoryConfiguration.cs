using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Data.Configurations;

/// <summary>
/// Category entity configuration
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.CategoryId);
        builder.Property(c => c.CategoryId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.CategoryName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(250);

        builder.Property(c => c.Picture)
            .HasColumnType("image");

        builder.Property(c => c.PictureLink)
            .HasMaxLength(100);

        // Index
        builder.HasIndex(c => c.CategoryName).HasDatabaseName("IX_Categories_CategoryName");

        // Seed data
        builder.HasData(
            new
            {
                CategoryId = "01JH179GGZ7FAHZ0DNFYNZ21BB",
                CategoryName = "Electronics",
                Description = "Electronic devices and accessories",
                PictureLink = "https://example.com/electronics.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                CategoryId = "01JH179GGZ7FAHZ0DNFYNZ23DD",
                CategoryName = "Mobile Phones",
                Description = "Smartphones and mobile accessories",
                PictureLink = "https://example.com/phones.jpg",
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                CategoryId = "01JH179GGZ7FAHZ0DNFYNZ24EE",
                CategoryName = "Computers",
                Description = "Laptops, desktops and computer accessories",
                PictureLink = "https://example.com/computers.jpg",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
