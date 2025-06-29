using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Persistence.Extensions;
namespace ProductManager.Application.Feature.Shipper.Commands;

public record AddOrUpdateShipperCommand(Shippers Shipper) : ICommand<ApiResponse>;
public class AddOrUpdateShipperCommandHandler : ICommandHandler<AddOrUpdateShipperCommand, ApiResponse>
{
    private readonly ICrudService<Shippers> _crudService;

    public AddOrUpdateShipperCommandHandler(ICrudService<Shippers> crudService)
    {
        _crudService = crudService;
    }

    public async Task<ApiResponse> HandleAsync(AddOrUpdateShipperCommand command, CancellationToken cancellationToken = default)
    {
        var shipper = command.Shipper;

        if (string.IsNullOrEmpty(shipper.Id))
        {
            shipper.Id = UlidExtension.Generate();
            await _crudService.AddAsync(shipper, cancellationToken);
            return new ApiResponse
            {
                StatusCode = 201,
                Message = CRUDMessage.CreateSuccess,
                Result = shipper
            };
        }

        await _crudService.UpdateAsync(shipper, cancellationToken);
        return new ApiResponse
        {
            StatusCode = 200,
            Message = CRUDMessage.UpdateSuccess,
            Result = shipper
        };
    }
}
