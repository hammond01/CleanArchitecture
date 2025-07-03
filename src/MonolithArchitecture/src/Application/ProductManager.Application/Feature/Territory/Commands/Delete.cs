using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Territory.Commands;

public class DeleteTerritoryCommand : ICommand<ApiResponse>
{
    public DeleteTerritoryCommand(Territories territory)
    {
        Territory = territory;
    }
    public Territories Territory { get; set; }
}

public class DeleteTerritoryCommandHandler : ICommandHandler<DeleteTerritoryCommand, ApiResponse>
{
    private readonly ICrudService<Territories> _territoryService;

    public DeleteTerritoryCommandHandler(ICrudService<Territories> territoryService)
    {
        _territoryService = territoryService;
    }

    public async Task<ApiResponse> HandleAsync(DeleteTerritoryCommand request, CancellationToken cancellationToken)
    {
        await _territoryService.DeleteAsync(request.Territory, cancellationToken);

        return new ApiResponse
        {
            StatusCode = 204,
            Message = CRUDMessage.DeleteSuccess
        };
    }
}
