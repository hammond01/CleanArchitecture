﻿namespace ProductManager.Persistence.MappingConfigurations;

public class ShipperConfiguration : IEntityTypeConfiguration<Shipper>
{
    public void Configure(EntityTypeBuilder<Shipper> builder)
    {
        builder.ToTable("Shippers");
        builder.HasKey(x => x.Id);
    }
}
