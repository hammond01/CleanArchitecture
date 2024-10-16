namespace ProductManager.Domain.DTOs.CategoryDto;

public class CreateCategoryDto
{
    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }
}
