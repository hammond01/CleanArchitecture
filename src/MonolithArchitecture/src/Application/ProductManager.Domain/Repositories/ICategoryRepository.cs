namespace ProductManager.Domain.Repositories;

public interface ICategoryRepository
{
    Task<ApiResponse> GetCategoriesAsync();
    Task<ApiResponse> GetCategoryAsync(String id);
    Task<ApiResponse> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
}
