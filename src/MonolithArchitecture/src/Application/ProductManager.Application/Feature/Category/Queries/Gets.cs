using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
namespace ProductManager.Application.Feature.Category.Queries;

public record GetCategories : IQuery<ApiResponse>;
internal class GetsCategoryHandler : IQueryHandler<GetCategories, ApiResponse>
{
    private readonly ICrudService<Categories> _categoryService;
    public GetsCategoryHandler(ICrudService<Categories> categoryService)
    {
        _categoryService = categoryService;
    }
    public async Task<ApiResponse> HandleAsync(GetCategories query, CancellationToken cancellationToken = default)
    {
        var response = await _categoryService.GetAsync();
        return new ApiResponse(200, "Get categories successfully", response);
    }
}
