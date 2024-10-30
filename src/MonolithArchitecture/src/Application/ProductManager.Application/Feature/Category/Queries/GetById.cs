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
    public GetCategoryByIdHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse> HandleAsync(GetCategoryById request, CancellationToken cancellationToken)
        => await _categoryRepository.GetCategoryAsync(request.MerchantId);
}
