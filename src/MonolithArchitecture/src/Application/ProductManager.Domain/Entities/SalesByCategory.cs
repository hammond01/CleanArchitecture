namespace ProductManager.Domain.Entities;

[Keyless]
public class SalesByCategory
{
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(15)]
    public string CategoryName { get; set; } = null!;

    [StringLength(40)]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "money")]
    public decimal? ProductSales { get; set; }
}
