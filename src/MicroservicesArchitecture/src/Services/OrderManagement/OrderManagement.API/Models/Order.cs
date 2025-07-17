using System.ComponentModel.DataAnnotations;

namespace OrderManagement.API.Models;

public class Order
{
    [Key]
    public int OrderId { get; set; }
    
    [Required]
    public string OrderNumber { get; set; } = string.Empty;
    
    [Required]
    public int CustomerId { get; set; }
    
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [Required]
    public DateTime OrderDate { get; set; }
    
    [Required]
    public OrderStatus Status { get; set; }
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    public string? ShippingAddress { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal UnitPrice { get; set; }
    
    [Required]
    public decimal TotalPrice { get; set; }
    
    // Navigation properties
    public virtual Order Order { get; set; } = null!;
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Refunded = 7
}
