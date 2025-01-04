namespace ProductManager.Persistence.MappingConfigurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{

    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);
    }
}
