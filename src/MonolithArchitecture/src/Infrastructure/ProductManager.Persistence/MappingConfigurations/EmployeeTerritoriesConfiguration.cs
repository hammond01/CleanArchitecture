using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;
namespace ProductManager.Persistence.MappingConfigurations;

public class EmployeeTerritoriesConfiguration : IEntityTypeConfiguration<EmployeeTerritory>
{
    public void Configure(EntityTypeBuilder<EmployeeTerritory> builder)
    {
        builder.ToTable("EmployeeTerritories");
        builder.HasKey(x => new
        {
            x.EmployeeId, x.TerritoryId
        });
        builder.HasOne(et => et.Employee)
            .WithMany(e => e.EmployeeTerritories)
            .HasForeignKey(et => et.EmployeeId);
        builder.HasOne(et => et.Territory)
            .WithMany(t => t.EmployeeTerritories)
            .HasForeignKey(et => et.TerritoryId);
    }
}
