using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
                Id = UlidExtension.Generate(),
                Description = "Category for the latest mobile phones",
                CategoryName = "Mobile Phones",
                CreatedDateTime = DateTimeOffset.Now
            },
            new Categories
            {
                Id = UlidExtension.Generate(),
                Description = "Category for accessories such as cases, chargers, cables",
                CategoryName = "Accessories",
                CreatedDateTime = DateTimeOffset.Now
            },
            new Categories
            {
                Id = UlidExtension.Generate(),
                Description = "Category for SIM cards and promotional plans",
                CategoryName = "SIM Cards",
                CreatedDateTime = DateTimeOffset.Now
            },
            new Categories
            {
                Id = UlidExtension.Generate(),
                Description = "Category for professional phone repair services",
                CategoryName = "Repair Services",
                CreatedDateTime = DateTimeOffset.Now
            },
            new Categories
            {
                Id = UlidExtension.Generate(),
                Description = "Category for extended warranty packages",
                CategoryName = "Extended Warranty",
                CreatedDateTime = DateTimeOffset.Now
            }
        });
    }
}
