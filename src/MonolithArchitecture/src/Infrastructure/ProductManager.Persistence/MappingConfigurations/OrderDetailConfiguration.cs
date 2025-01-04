namespace ProductManager.Persistence.MappingConfigurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{

    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("OrderDetails");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UnitPrice).HasColumnType("money");
        builder.Property(x => x.Quantity).HasColumnType("smallint");
        builder.Property(x => x.Discount).HasColumnType("real");
        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderDetails)
            .HasForeignKey(x => x.Id);
        builder.HasOne(x => x.Products)
            .WithMany(x => x.OrderDetails)
            .HasForeignKey(x => x.Id);
    }
}
