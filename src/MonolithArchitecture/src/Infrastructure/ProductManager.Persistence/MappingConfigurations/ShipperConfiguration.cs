using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;
namespace ProductManager.Persistence.MappingConfigurations;

public class ShipperConfiguration : IEntityTypeConfiguration<Shippers>
{
    public void Configure(EntityTypeBuilder<Shippers> builder)
    {
        builder.ToTable("Shippers");
        builder.HasKey(x => x.Id);
    }
}
