using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;
using ProductManager.Persistence.Extensions;
namespace ProductManager.Persistence.MappingConfigurations;

public class RegionConfiguration : IEntityTypeConfiguration<Regions>
{

    public void Configure(EntityTypeBuilder<Regions> builder)
    {
        builder.ToTable("Region");
        builder.HasKey(x => x.Id);

        builder.HasData(new List<Regions>
        {
            new Regions
            {
                Id = UlidExtension.Generate(), RegionDescription = "Ha Noi", TestCode = "TestCode1"
            },
            new Regions
            {
                Id = UlidExtension.Generate(), RegionDescription = "Ho Chi Minh City", TestCode = "TestCode2"
            },
            new Regions
            {
                Id = UlidExtension.Generate(), RegionDescription = "Can Tho", TestCode = "TestCode3"
            },
            new Regions
            {
                Id = UlidExtension.Generate(), RegionDescription = "Nha Trang", TestCode = "TestCode4"
            }
        });
    }
}
