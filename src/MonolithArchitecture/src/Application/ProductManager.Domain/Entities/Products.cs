namespace ProductManager.Domain.Entities;

[Index("CategoryId", Name = "CategoriesProducts")]
[Index("CategoryId", Name = "CategoryID")]
[Index("ProductName", Name = "ProductName")]
[Index("SupplierId", Name = "SupplierID")]
[Index("SupplierId", Name = "SuppliersProducts")]
public class Products : Entity<string>
{
    [StringLength(40)]
    public string ProductName { get; set; } = null!;

    [Column("SupplierID")]
    public string SupplierId { get; set; } = null!;

    [Column("CategoryID")]
    public string CategoryId { get; set; } = null!;

    [StringLength(20)]
    public string? QuantityPerUnit { get; set; }

    [Column(TypeName = "money")]
    public decimal? UnitPrice { get; set; }

    public short? UnitsInStock { get; set; }

    public short? UnitsOnOrder { get; set; }

    public short? ReorderLevel { get; set; }

    public bool Discontinued { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public Categories Category { get; set; } = null!;

    [InverseProperty("Products")]
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("SupplierId")]
    [InverseProperty("Products")]
    public Suppliers Supplier { get; set; } = null!;
}
