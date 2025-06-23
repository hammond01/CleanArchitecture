using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Product.Queries;

public record GetProducts : IQuery<ApiResponse>;
internal class GetsProductsHandler : IQueryHandler<GetProducts, ApiResponse>
{
    private readonly ICrudService<Products> _crudService;
    public GetsProductsHandler(ICrudService<Products> crudService)
    {
        _crudService = crudService;
    }
    public async Task<ApiResponse> HandleAsync(GetProducts query, CancellationToken cancellationToken = default)
    {
        var response = await _crudService.GetAsync();
        return new ApiResponse(200, "Get product successfully", response);
    }
}
