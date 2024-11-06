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
        _categoryService = categoryService;
    }

    public async Task<ApiResponse> HandleAsync(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetByIdAsync(request.CategoryId, cancellationToken);
        return categories == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, "Get data by Id successfully", categories);
    }
}
