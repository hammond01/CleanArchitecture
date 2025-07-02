using System.Data;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Persistence.Extensions;

namespace ProductManager.Application.Feature.Region.Commands;

public class AddOrUpdateRegionCommand : ICommand<ApiResponse>
{
    public AddOrUpdateRegionCommand(Regions region)
    {
        Region = region;
    }
    public Regions Region { get; set; }
}

internal class AddOrUpdateRegionHandler : ICommandHandler<AddOrUpdateRegionCommand, ApiResponse>
{
    private readonly ICrudService<Regions> _regionService;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrUpdateRegionHandler(ICrudService<Regions> regionService, IUnitOfWork unitOfWork)
    {
        _regionService = regionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> HandleAsync(AddOrUpdateRegionCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            if (command.Region.Id == null!)
            {
                command.Region.Id = UlidExtension.Generate();
                await _regionService.AddAsync(command.Region, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new ApiResponse(201, CRUDMessage.CreateSuccess, command.Region);
            }
            await _regionService.UpdateAsync(command.Region, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, CRUDMessage.UpdateSuccess);
        }
    }
}
