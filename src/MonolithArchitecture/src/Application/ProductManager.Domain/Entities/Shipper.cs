namespace ProductManager.Domain.Entities;

public class Shipper : Entity<int>
{
    [StringLength(40)]
    public string CompanyName { get; set; } = null!;

    [StringLength(24)]
    public string? Phone { get; set; }

    [InverseProperty("ShipViaNavigation")]
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
