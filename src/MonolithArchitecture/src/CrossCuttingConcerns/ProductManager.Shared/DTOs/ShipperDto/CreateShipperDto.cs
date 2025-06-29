using System.ComponentModel.DataAnnotations;

namespace ProductManager.Shared.DTOs.ShipperDto;

public class CreateShipperDto
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(40, ErrorMessage = "Company name cannot exceed 40 characters")]
    public string CompanyName { get; set; } = null!;

    [StringLength(24, ErrorMessage = "Phone cannot exceed 24 characters")]
    public string? Phone { get; set; }
}
