namespace ProductManager.Persistence.MappingConfigurations;

public class TerritoryConfiguration : IEntityTypeConfiguration<Territory>
{

    public void Configure(EntityTypeBuilder<Territory> builder)
    {
        builder.ToTable("Territories");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Region)
            .WithMany(x => x.Territories)
            .HasForeignKey(x => x.RegionId);
    }
}
