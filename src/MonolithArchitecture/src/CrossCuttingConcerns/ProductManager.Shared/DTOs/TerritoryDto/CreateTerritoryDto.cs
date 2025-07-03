using System.ComponentModel.DataAnnotations;

namespace ProductManager.Shared.DTOs.TerritoryDto;

public class CreateTerritoryDto
{
    [Required]
    [StringLength(50)]
    public string Id { get; set; } = default!;

    [Required]
    [StringLength(50)]
    public string TerritoryDescription { get; set; } = default!;

    [Required]
    [StringLength(50)]
    public string RegionId { get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = default!;
}
