using System.Threading;
using System.Threading.Tasks;
using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

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
        var categories = await _crudService.GetByIdAsync(request.ProductId);
        return categories == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, "Get data by Id successfully", categories);
    }
}
