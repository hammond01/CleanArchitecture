namespace ProductManager.Domain.Entities;

[Keyless]
public class CurrentProductList
{
    [Column("ProductID")]
    public int ProductId { get; set; }

    [StringLength(40)]
    public string ProductName { get; set; } = null!;
}
