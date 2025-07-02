using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Region.Queries;

public record GetRegions : IQuery<ApiResponse>;

internal class GetRegionsHandler : IQueryHandler<GetRegions, ApiResponse>
{
    private readonly ICrudService<Regions> _regionService;

    public GetRegionsHandler(ICrudService<Regions> regionService)
    {
        _regionService = regionService;
    }

    public async Task<ApiResponse> HandleAsync(GetRegions query, CancellationToken cancellationToken = default)
    {
        var response = await _regionService.GetAsync();
        return new ApiResponse(200, "Get regions successfully", response);
    }
}
