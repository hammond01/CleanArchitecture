namespace ProductManager.Persistence.MappingConfigurations;

public class ProductsConfiguration : IEntityTypeConfiguration<Products>
{

    public void Configure(EntityTypeBuilder<Products> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);
    }
}
