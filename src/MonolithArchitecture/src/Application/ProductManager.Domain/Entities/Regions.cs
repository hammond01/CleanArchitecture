using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProductManager.Domain.Entities;

[Table("Region")]
public class Regions : Entity<string>
{
    [StringLength(50)]
    public string RegionDescription { get; set; } = null!;

    [StringLength(50)]
    public string TestCode { get; set; } = null!;

    [InverseProperty("Region")]
    public ICollection<Territory> Territories { get; set; } = new List<Territory>();
}
