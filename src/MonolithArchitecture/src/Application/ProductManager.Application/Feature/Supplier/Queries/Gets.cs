using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
namespace ProductManager.Application.Feature.Supplier.Queries;

public record GetSuppliers : IQuery<ApiResponse>;
internal class GetsSupplierHandler : IQueryHandler<GetSuppliers, ApiResponse>
{
    private readonly ICrudService<Suppliers> _crudService;
    public GetsSupplierHandler(ICrudService<Suppliers> crudService)
    {
        _crudService = crudService;
    }
    public async Task<ApiResponse> HandleAsync(GetSuppliers query, CancellationToken cancellationToken = default)
    {
        var response = await _crudService.GetAsync();
        return new ApiResponse(200, CRUDMessage.GetSuccess, response);
    }
}
