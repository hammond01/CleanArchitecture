using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;
namespace ProductManager.Persistence.MappingConfigurations;

public class TerritoryConfiguration : IEntityTypeConfiguration<Territories>
{

    public void Configure(EntityTypeBuilder<Territories> builder)
    {
        builder.ToTable("Territories");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Region)
            .WithMany(x => x.Territories)
            .HasForeignKey(x => x.RegionId);
    }
}
