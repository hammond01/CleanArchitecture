using BuildingBlocks.Domain.Entities;

namespace Catalog.Domain.Entities;

/// <summary>
/// Category entity
/// </summary>
public class Category : Entity<string>
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public byte[]? Picture { get; set; }
    public string? PictureLink { get; set; }

    // Navigation property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
