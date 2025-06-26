using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;
using ProductManager.Persistence.Extensions;
namespace ProductManager.Persistence.MappingConfigurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Suppliers>
{
    public void Configure(EntityTypeBuilder<Suppliers> builder)
    {
        builder.ToTable("Suppliers");
        builder.HasKey(x => x.Id);

        builder.HasData(new List<Suppliers>
        {
            new Suppliers
            {
                Id = "01JH179GGZ7FAHZ0DNFYNZ18YX",
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
                HomePage = "https://github.com/hammond01"
            },
            new Suppliers
            {
                Id = "01JH179GGZ7FAHZ0DNFYNZ19FG",
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
                HomePage = "https://github.com/hammond01"
            }
        });
    }
}
