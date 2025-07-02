using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Region.Queries;

public record GetRegionByIdQuery : IQuery<ApiResponse>
{
    public GetRegionByIdQuery(string regionId)
    {
        RegionId = regionId;
    }
    public string RegionId { get; set; }
}

public class GetRegionByIdHandler : IQueryHandler<GetRegionByIdQuery, ApiResponse>
{
    private readonly ICrudService<Regions> _regionService;

    public GetRegionByIdHandler(ICrudService<Regions> regionService)
    {
        _regionService = regionService;
    }

    public async Task<ApiResponse> HandleAsync(GetRegionByIdQuery request, CancellationToken cancellationToken)
    {
        var region = await _regionService.GetByIdAsync(request.RegionId);
        return region == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, "Get data by Id successfully", region);
    }
}
