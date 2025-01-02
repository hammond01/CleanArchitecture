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

        builder.HasData(new List<Territory>
        {
            new Territory
            {
                Id = UlidExtension.Generate(), TerritoryDescription = "North", RegionId = "1"
            },
            new Territory
            {
                Id = UlidExtension.Generate(), TerritoryDescription = "South", RegionId = "1"
            },
            new Territory
            {
                Id = UlidExtension.Generate(), TerritoryDescription = "East", RegionId = "2"
            },
            new Territory
            {
                Id = UlidExtension.Generate(), TerritoryDescription = "West", RegionId = "2"
            }
        });
    }
}
