namespace ProductManager.Application.Feature.Category.Queries;

public record GetCategories : IQuery<ApiResponse>;
internal class GetsCategoryHandler : IQueryHandler<GetCategories, ApiResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    public GetsCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<ApiResponse> HandleAsync(GetCategories query, CancellationToken cancellationToken = default)
        => await _categoryRepository.GetCategoriesAsync();
}
