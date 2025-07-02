using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Region.Commands;

public class DeleteRegionCommand : ICommand<ApiResponse>
{
    public DeleteRegionCommand(Regions region)
    {
        Region = region;
    }
    public Regions Region { get; set; }
}

public class DeleteRegionCommandHandler : ICommandHandler<DeleteRegionCommand, ApiResponse>
{
    private readonly ICrudService<Regions> _regionService;

    public DeleteRegionCommandHandler(ICrudService<Regions> regionService)
    {
        _regionService = regionService;
    }

    public async Task<ApiResponse> HandleAsync(DeleteRegionCommand request, CancellationToken cancellationToken)
    {
        await _regionService.DeleteAsync(request.Region, cancellationToken);

        return new ApiResponse
        {
            Message = CRUDMessage.DeleteSuccess
        };
    }
}
