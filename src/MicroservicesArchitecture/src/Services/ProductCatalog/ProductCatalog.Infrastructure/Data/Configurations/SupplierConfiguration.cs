using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Data.Configurations;

/// <summary>
/// Supplier entity configuration
/// </summary>
public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.SupplierId);
        builder.Property(s => s.SupplierId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.CompanyName)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(s => s.ContactName)
            .HasMaxLength(30);

        builder.Property(s => s.ContactTitle)
            .HasMaxLength(30);

        builder.Property(s => s.Address)
            .HasMaxLength(60);

        builder.Property(s => s.City)
            .HasMaxLength(15);

        builder.Property(s => s.Region)
            .HasMaxLength(15);

        builder.Property(s => s.PostalCode)
            .HasMaxLength(10);

        builder.Property(s => s.Country)
            .HasMaxLength(15);

        builder.Property(s => s.Phone)
            .HasMaxLength(24);

        builder.Property(s => s.Fax)
            .HasMaxLength(24);

        builder.Property(s => s.HomePage);

        // Indexes
        builder.HasIndex(s => s.CompanyName).HasDatabaseName("IX_Suppliers_CompanyName");
        builder.HasIndex(s => s.PostalCode).HasDatabaseName("IX_Suppliers_PostalCode");

        // Seed data
        builder.HasData(
            new
            {
                SupplierId = "01JH179GGZ7FAHZ0DNFYNZ18YX",
                CompanyName = "Tech Supplies Co.",
                ContactName = "John Doe",
                ContactTitle = "Sales Manager",
                Address = "123 Tech Street",
                City = "TechCity",
                Region = "TechRegion",
                PostalCode = "12345",
                Country = "Techland",
                Phone = "123-456-7890",
                Fax = "123-456-7891",
                HomePage = "https://github.com/hammond01",
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                SupplierId = "01JH179GGZ7FAHZ0DNFYNZ19FG",
                CompanyName = "Mobile Accessories Inc.",
                ContactName = "Jane Smith",
                ContactTitle = "CEO",
                Address = "456 Mobile Blvd",
                City = "MobileCity",
                Region = "MobileRegion",
                PostalCode = "67890",
                Country = "PhoneCountry",
                Phone = "098-765-4321",
                Fax = "098-765-4322",
                HomePage = "https://github.com/hammond01",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
