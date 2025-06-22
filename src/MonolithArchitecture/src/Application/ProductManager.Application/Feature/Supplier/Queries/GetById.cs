using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
namespace ProductManager.Application.Feature.Supplier.Queries;

public record GetSupplierByIdQuery : IQuery<ApiResponse>
{
    public GetSupplierByIdQuery(string categoryId)
    {
        SupplierId = categoryId;
    }
    public string SupplierId { get; set; }
}
public class GetSupplierByIdHandler : IQueryHandler<GetSupplierByIdQuery, ApiResponse>
{
    private readonly ICrudService<Suppliers> _crudService;
    public GetSupplierByIdHandler(ICrudService<Suppliers> crudService)
    {
        _crudService = crudService;
    }

    public async Task<ApiResponse> HandleAsync(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var categories = await _crudService.GetByIdAsync(request.SupplierId);
        return categories == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, CRUDMessage.GetSuccess, categories);
    }
}
