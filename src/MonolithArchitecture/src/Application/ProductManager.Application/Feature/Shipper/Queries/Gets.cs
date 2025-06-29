using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Shipper.Queries;

public record GetShippers : IQuery<ApiResponse>;

public class GetShippersQueryHandler : IQueryHandler<GetShippers, ApiResponse>
{
    private readonly ICrudService<Shippers> _crudService;

    public GetShippersQueryHandler(ICrudService<Shippers> crudService)
    {
        _crudService = crudService;
    }

    public async Task<ApiResponse> HandleAsync(GetShippers query, CancellationToken cancellationToken = default)
    {
        var shippers = await _crudService.GetAsync();
        return new ApiResponse
        {
            StatusCode = 200,
            Message = "Shippers retrieved successfully",
            Result = shippers
        };
    }
}
