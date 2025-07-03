using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProductManager.Domain.Entities;

public class Territories : Entity<string>
{
    [StringLength(50)]
    public string TerritoryDescription { get; set; } = null!;

    [Column("RegionID")]
    public string RegionId { get; set; } = default!;

    [ForeignKey("RegionId")]
    [InverseProperty("Territories")]
    public Regions Region { get; set; } = null!;

    public ICollection<EmployeeTerritory> EmployeeTerritories { get; set; } = new List<EmployeeTerritory>();
}
