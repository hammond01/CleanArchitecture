namespace ProductManager.Domain.Entities;

[Keyless]
public class SalesTotalsByAmount
{
    [Column(TypeName = "money")]
    public decimal? SaleAmount { get; set; }

    [Column("OrderID")]
    public int OrderId { get; set; }

    [StringLength(40)]
    public string CompanyName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ShippedDate { get; set; }
}
