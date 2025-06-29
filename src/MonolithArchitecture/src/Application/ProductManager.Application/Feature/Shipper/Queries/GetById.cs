using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Shipper.Queries;

public record GetShipperByIdQuery : IQuery<ApiResponse>
{
    public string ShipperId { get; set; } = null!;
}

public class GetShipperByIdQueryHandler : IQueryHandler<GetShipperByIdQuery, ApiResponse>
{
    private readonly ICrudService<Shippers> _crudService;

    public GetShipperByIdQueryHandler(ICrudService<Shippers> crudService)
    {
        _crudService = crudService;
    }

    public async Task<ApiResponse> HandleAsync(GetShipperByIdQuery query, CancellationToken cancellationToken = default)
    {
        var shipper = await _crudService.GetByIdAsync(query.ShipperId);
        if (shipper == null)
        {
            return new ApiResponse
            {
                StatusCode = 404,
                Message = CRUDMessage.GetFailed,
                Result = null!
            };
        }

        return new ApiResponse
        {
            StatusCode = 200,
            Message = "Shipper retrieved successfully",
            Result = shipper
        };
    }
}
