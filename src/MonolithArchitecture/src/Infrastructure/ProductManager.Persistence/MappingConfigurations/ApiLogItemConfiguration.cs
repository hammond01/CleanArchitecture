namespace ProductManager.Persistence.MappingConfigurations;

public class ApiLogItemConfiguration : IEntityTypeConfiguration<ApiLogItem>
{

    public void Configure(EntityTypeBuilder<ApiLogItem> builder)
    {
        builder.ToTable("ApiLogs");
        builder.HasKey(x => x.Id);
    }
}
