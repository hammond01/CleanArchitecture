namespace ProductManager.Domain.Entities;

[Keyless]
public class CategorySalesFor1997
{
    [StringLength(15)]
    public string CategoryName { get; set; } = null!;

    [Column(TypeName = "money")]
    public decimal? CategorySales { get; set; }
}
