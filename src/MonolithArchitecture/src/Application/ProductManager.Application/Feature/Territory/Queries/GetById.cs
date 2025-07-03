using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Territory.Queries;

public record GetTerritoryByIdQuery : IQuery<ApiResponse>
{
    public GetTerritoryByIdQuery(string territoryId)
    {
        TerritoryId = territoryId;
    }
    public string TerritoryId { get; set; }
}

public class GetTerritoryByIdHandler : IQueryHandler<GetTerritoryByIdQuery, ApiResponse>
{
    private readonly ICrudService<Territories> _territoryService;

    public GetTerritoryByIdHandler(ICrudService<Territories> territoryService)
    {
        _territoryService = territoryService;
    }

    public async Task<ApiResponse> HandleAsync(GetTerritoryByIdQuery request, CancellationToken cancellationToken)
    {
        var territory = await _territoryService.GetByIdAsync(request.TerritoryId);
        return territory == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, "Get data by Id successfully", territory);
    }
}
