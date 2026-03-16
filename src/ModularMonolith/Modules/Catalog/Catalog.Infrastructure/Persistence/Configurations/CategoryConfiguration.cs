using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", "catalog");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CategoryName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Description)
            .HasMaxLength(250);

        builder.Property(c => c.Picture)
            .HasColumnType("image");

        builder.Property(c => c.PictureLink)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(c => c.CategoryName).HasDatabaseName("CategoryName");

        // Relationships configured in Product
    }
}
