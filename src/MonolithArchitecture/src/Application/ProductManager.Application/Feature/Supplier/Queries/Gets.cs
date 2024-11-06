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
        var response = await _crudService.GetAsync(cancellationToken);
        return new ApiResponse(200, "Get suppliers successfully", response);
    }
}
