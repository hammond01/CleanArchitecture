﻿namespace ProductManager.Domain.DTOs.CategoryDto;

public class GetCategoryDto
{
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
}