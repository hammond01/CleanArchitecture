using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagement.Domain.Entities;

public class Order : Entity<int>
{
  [StringLength(5)]
  public string CustomerId { get; set; } = null!;

  public DateTime? OrderDate { get; set; }

  public DateTime? RequiredDate { get; set; }

  public DateTime? ShippedDate { get; set; }

  public decimal? Freight { get; set; }

  [StringLength(40)]
  public string? ShipName { get; set; }

  [StringLength(60)]
  public string? ShipAddress { get; set; }

  [StringLength(15)]
  public string? ShipCity { get; set; }

  [StringLength(15)]
  public string? ShipRegion { get; set; }

  [StringLength(10)]
  public string? ShipPostalCode { get; set; }

  [StringLength(15)]
  public string? ShipCountry { get; set; }

  // Navigation property
  public Customer Customer { get; set; } = null!;
}
