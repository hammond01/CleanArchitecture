using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;
using ProductManager.Persistence.Extensions;
namespace ProductManager.Persistence.MappingConfigurations;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{

    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("Region");
        builder.HasKey(x => x.Id);

        builder.HasData(new List<Region>
        {
            new Region
            {
                Id = UlidExtension.Generate(), RegionDescription = "Ha Noi"
            },
            new Region
            {
                Id = UlidExtension.Generate(), RegionDescription = "Ho Chi Minh City"
            },
            new Region
            {
                Id = UlidExtension.Generate(), RegionDescription = "Can Tho"
            },
            new Region
            {
                Id = UlidExtension.Generate(), RegionDescription = "Nha Trang"
            }
        });
    }
}
