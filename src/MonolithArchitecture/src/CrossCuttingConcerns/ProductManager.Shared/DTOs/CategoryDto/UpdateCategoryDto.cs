namespace ProductManager.Shared.DTOs.CategoryDto;

public class UpdateCategoryDto
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
}
