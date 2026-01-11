namespace Catalog.Application.DTOs;

/// <summary>
/// Category Data Transfer Object
/// </summary>
public class CategoryDto
{
    public string Id { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public string? PictureLink { get; set; }
    public int ProductCount { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
