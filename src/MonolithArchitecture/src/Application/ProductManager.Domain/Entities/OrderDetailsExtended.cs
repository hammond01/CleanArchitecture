namespace ProductManager.Domain.Entities;

[Keyless]
public class OrderDetailsExtended
{
    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("ProductID")]
    public int ProductId { get; set; }

    [StringLength(40)]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "money")]
    public decimal UnitPrice { get; set; }

    public short Quantity { get; set; }

    public float Discount { get; set; }

    [Column(TypeName = "money")]
    public decimal? ExtendedPrice { get; set; }
}
