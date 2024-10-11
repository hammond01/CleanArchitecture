namespace ProductManager.Domain.Entities;

[Keyless]
public class ProductSalesFor1997
{
    [StringLength(15)]
    public string CategoryName { get; set; } = null!;

    [StringLength(40)]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "money")]
    public decimal? ProductSales { get; set; }
}
