﻿using ProductManager.Domain.Common;
using ProductManager.Domain.DTOs.CategoryDto;
namespace ProductManager.Domain.Repositories;

public interface ICategoryRepository
{
    Task<ApiResponse> GetCategoriesAsync();
    Task<ApiResponse> GetCategoryAsync(int id);
    Task<ApiResponse> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
}