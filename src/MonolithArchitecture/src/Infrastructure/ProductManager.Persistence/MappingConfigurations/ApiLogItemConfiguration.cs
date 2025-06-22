using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;
namespace ProductManager.Persistence.MappingConfigurations;

public class ApiLogItemConfiguration : IEntityTypeConfiguration<ApiLogItem>
{
    public void Configure(EntityTypeBuilder<ApiLogItem> builder)
    {
        builder.ToTable("ApiLogs");
        builder.HasKey(x => x.Id);        // Configure DateTime property to be stored properly
        builder.Property(x => x.RequestTime)
               .HasColumnType("datetime2")
               .IsRequired();

        // Configure required relationship with User via UserId
        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .IsRequired(true).OnDelete(DeleteBehavior.Cascade);

        // ApplicationUserId is additional nullable field
        builder.Property(x => x.ApplicationUserId)
               .IsRequired(false);
    }
}
