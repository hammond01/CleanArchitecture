using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Product.Queries;

public record GetProductByIdQuery : IQuery<ApiResponse>
{
    public GetProductByIdQuery(string productId)
    {
        ProductId = productId;
    }
    public string ProductId { get; set; }
}
public class GetProductByIdHandler : IQueryHandler<GetProductByIdQuery, ApiResponse>
{
    private readonly ICrudService<Products> _crudService;
    public GetProductByIdHandler(ICrudService<Products> productService)
    {
        _crudService = productService;
    }

    public async Task<ApiResponse> HandleAsync(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _crudService.GetByIdAsync(request.ProductId);
        return product == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, "Get data by Id successfully", product);
    }
}
