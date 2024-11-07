namespace ProductManager.Application.Feature.Product.Queries;

public record GetProductByIdQuery : IQuery<ApiResponse>
{
    public GetProductByIdQuery(string categoryId)
    {
        ProductId = categoryId;
    }
    public string ProductId { get; set; }
}
public class GetProductByIdHandler : IQueryHandler<GetProductByIdQuery, ApiResponse>
{
    private readonly ICrudService<Products> _crudService;
    public GetProductByIdHandler(ICrudService<Products> categoryService)
    {
        _crudService = categoryService;
    }

    public async Task<ApiResponse> HandleAsync(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var categories = await _crudService.GetByIdAsync(request.ProductId, cancellationToken);
        return categories == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, "Get data by Id successfully", categories);
    }
}
