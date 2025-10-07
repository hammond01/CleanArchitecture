using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Category.Queries;

public record GetCategoryByIdQuery : IQuery<ApiResponse>
{
    public GetCategoryByIdQuery(string categoryId)
    {
        CategoryId = categoryId;
    }
    public string CategoryId { get; set; }
}
public class GetCategoryByIdHandler : IQueryHandler<GetCategoryByIdQuery, ApiResponse>
{
    private readonly ICrudService<Categories> _categoryService;
    public GetCategoryByIdHandler(ICrudService<Categories> categoryService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    public async Task<ApiResponse> HandleAsync(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetByIdAsync(request.CategoryId);
        return categories == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, "Get data by Id successfully", categories);
    }
}
