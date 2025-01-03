namespace ProductManager.Persistence.MappingConfigurations;

public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.ToTable("OutboxEvents");
        builder.HasIndex(x => x.Published);
        builder.HasIndex(x => x.CreatedDateTime);
    }
}
public class ArchivedOutboxEventConfiguration : IEntityTypeConfiguration<ArchivedOutboxEvent>
{
    public void Configure(EntityTypeBuilder<ArchivedOutboxEvent> builder)
    {
        builder.ToTable("ArchivedOutboxEvents");
        builder.HasIndex(x => x.CreatedDateTime);
    }
}
