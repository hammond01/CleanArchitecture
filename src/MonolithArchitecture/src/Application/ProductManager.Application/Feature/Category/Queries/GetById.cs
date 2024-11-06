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
    private readonly IMapper _mapper;
    public GetCategoryByIdHandler(ICrudService<Categories> categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<ApiResponse> HandleAsync(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetByIdAsync(request.CategoryId, cancellationToken);
        var response = _mapper.Map<GetCategoryDto>(categories);
        return new ApiResponse(200, "Get Category by Id successfully", response);
    }
}
