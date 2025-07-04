using System.ComponentModel.DataAnnotations;

namespace ProductManager.Shared.DTOs.OrderDetailDto;

public class CreateOrderDetailDto
{
    [Required]
    [StringLength(50)]
    public string Id { get; set; } = default!;

    [Required]
    [StringLength(50)]
    public string ProductId { get; set; } = default!;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }

    [Required]
    [Range(1, short.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public short Quantity { get; set; }

    [Range(0, 1, ErrorMessage = "Discount must be between 0 and 1")]
    public float Discount { get; set; } = 0;
}
