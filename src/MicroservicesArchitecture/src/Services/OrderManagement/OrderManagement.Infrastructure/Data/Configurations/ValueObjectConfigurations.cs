using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for Money value object
/// </summary>
public class MoneyConfiguration : IEntityTypeConfiguration<Money>
{
    public void Configure(EntityTypeBuilder<Money> builder)
    {
        // This is handled in the owning entity configurations
        // This class exists for consistency but doesn't need implementation
        // since Money is always owned by other entities
    }
}

/// <summary>
/// Configuration for Address value object
/// </summary>
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        // This is handled in the owning entity configurations
        // This class exists for consistency but doesn't need implementation
        // since Address is always owned by other entities
    }
}

/// <summary>
/// Configuration for CustomerInfo value object
/// </summary>
public class CustomerInfoConfiguration : IEntityTypeConfiguration<CustomerInfo>
{
    public void Configure(EntityTypeBuilder<CustomerInfo> builder)
    {
        // This is handled in the owning entity configurations
        // This class exists for consistency but doesn't need implementation
        // since CustomerInfo is always owned by other entities
    }
}
