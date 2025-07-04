namespace ProductManager.Shared.DTOs.OrderDetailDto;

public class GetOrderDetailDto
{
    public string Id { get; set; } = default!;
    public string ProductId { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public short Quantity { get; set; }
    public float Discount { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity * (1 - (decimal)Discount);
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
