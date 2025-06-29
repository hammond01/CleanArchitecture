using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Shipper.Commands;

public record DeleteShipperCommand(Shippers Shipper) : ICommand<ApiResponse>;

public class DeleteShipperCommandHandler : ICommandHandler<DeleteShipperCommand, ApiResponse>
{
    private readonly ICrudService<Shippers> _crudService;

    public DeleteShipperCommandHandler(ICrudService<Shippers> crudService)
    {
        _crudService = crudService;
    }

    public async Task<ApiResponse> HandleAsync(DeleteShipperCommand command, CancellationToken cancellationToken = default)
    {
        var shipper = command.Shipper;
        await _crudService.DeleteAsync(shipper, cancellationToken);

        return new ApiResponse
        {
            StatusCode = 200,
            Message = CRUDMessage.DeleteSuccess,
            Result = null!
        };
    }
}
