namespace ProductManager.Domain.Entities;

public class Territory
{
    [Key]
    [Column("TerritoryID")]
    [StringLength(20)]
    public string TerritoryId { get; set; } = null!;

    [StringLength(50)]
    public string TerritoryDescription { get; set; } = null!;

    [Column("RegionID")]
    public int RegionId { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("Territories")]
    public Region Region { get; set; } = null!;

    [ForeignKey("TerritoryId")]
    [InverseProperty("Territories")]
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
