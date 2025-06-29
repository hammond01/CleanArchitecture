using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;
namespace ProductManager.Persistence.MappingConfigurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Categories>
{
    public void Configure(EntityTypeBuilder<Categories> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(x => x.Id);

        builder.HasData(new List<Categories>
        {
            new Categories
            {
                Id = "01JH179GGG9BN2V8SS9RG70QNG",
                Description = "Category for the latest mobile phones",
                CategoryName = "Mobile Phones",
                CreatedDateTime = DateTimeOffset.Now
            },
            new Categories
            {
                Id = "01JH179GGG9BN2V8SS9RG70QPX",
                Description = "Category for accessories such as cases, chargers, cables",
                CategoryName = "Accessories",
                CreatedDateTime = DateTimeOffset.Now
            },
            new Categories
            {
                Id = "01JH179GGG9BN2V8SS9RG70QRS",
                Description = "Category for SIM cards and promotional plans",
                CategoryName = "SIM Cards",
                CreatedDateTime = DateTimeOffset.Now
            },
            new Categories
            {
                Id = "01JH179GGG9BN2V8SS9RG70QTU",
                Description = "Category for professional phone repair services",
                CategoryName = "Repair Services",
                CreatedDateTime = DateTimeOffset.Now
            },
            new Categories
            {
                Id = "01JH179GGG9BN2V8SS9RG70QVW",
                Description = "Category for extended warranty packages",
                CategoryName = "Extended Warranty",
                CreatedDateTime = DateTimeOffset.Now
            }
        });
    }
}
