using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Territory.Queries;

public record GetTerritories : IQuery<ApiResponse>;

internal class GetTerritoriesHandler : IQueryHandler<GetTerritories, ApiResponse>
{
    private readonly ICrudService<Territories> _territoryService;

    public GetTerritoriesHandler(ICrudService<Territories> territoryService)
    {
        _territoryService = territoryService;
    }

    public async Task<ApiResponse> HandleAsync(GetTerritories query, CancellationToken cancellationToken = default)
    {
        var response = await _territoryService.GetAsync();
        return new ApiResponse(200, "Get territories successfully", response);
    }
}
