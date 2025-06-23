using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;
namespace ProductManager.Persistence.MappingConfigurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customers>
{
    public void Configure(EntityTypeBuilder<Customers> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(x => x.Id);
    }
}
