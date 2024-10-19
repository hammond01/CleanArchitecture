namespace ProductManager.Domain.Entities;

[PrimaryKey("OrderId", "ProductId")]
[Table("Order Details")]
[Index("OrderId", Name = "OrderID")]
[Index("OrderId", Name = "OrdersOrder_Details")]
[Index("ProductId", Name = "ProductID")]
[Index("ProductId", Name = "ProductsOrder_Details")]
public class OrderDetail : Entity<int>
{
    [Key]
    [Column("ProductID")]
    public int ProductId { get; set; }

    [Column(TypeName = "money")]
    public decimal UnitPrice { get; set; }

    public short Quantity { get; set; }

    public float Discount { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderDetails")]
    public Order Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("OrderDetails")]
    public Product Product { get; set; } = null!;
}
