using ProductManager.Application.Common.Services;
namespace ProductManager.Application.Feature.Category.Queries;

public record GetCategoryById : IQuery<ApiResponse>
{
    public GetCategoryById(string merchantId)
    {
        MerchantId = merchantId;
    }
    public string MerchantId { get; set; }
}
public class GetCategoryByIdHandler : IQueryHandler<GetCategoryById, ApiResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CrudService<Categories> _categoryService;
    public GetCategoryByIdHandler(ICategoryRepository categoryRepository, CrudService<Categories> categoryService)
    {
        _categoryRepository = categoryRepository;
        _categoryService = categoryService;
    }

    public async Task<ApiResponse> HandleAsync(GetCategoryById request, CancellationToken cancellationToken)
    {
        var response = await _categoryService.GetByIdAsync(request.MerchantId, cancellationToken);
        return new ApiResponse(200, "Get Category by Id successfully", response);
    }
}
