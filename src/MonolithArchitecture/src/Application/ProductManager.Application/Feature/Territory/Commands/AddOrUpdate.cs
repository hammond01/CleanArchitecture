using System.Data;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Persistence.Extensions;

namespace ProductManager.Application.Feature.Territory.Commands;

public class AddOrUpdateTerritoryCommand : ICommand<ApiResponse>
{
    public AddOrUpdateTerritoryCommand(Territories territory)
    {
        Territory = territory;
    }
    public Territories Territory { get; set; }
}

internal class AddOrUpdateTerritoryHandler : ICommandHandler<AddOrUpdateTerritoryCommand, ApiResponse>
{
    private readonly ICrudService<Territories> _territoryService;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrUpdateTerritoryHandler(ICrudService<Territories> territoryService, IUnitOfWork unitOfWork)
    {
        _territoryService = territoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> HandleAsync(AddOrUpdateTerritoryCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            if (command.Territory.Id == null!)
            {
                command.Territory.Id = UlidExtension.Generate();
                await _territoryService.AddAsync(command.Territory, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new ApiResponse(201, CRUDMessage.CreateSuccess, command.Territory);
            }
            await _territoryService.UpdateAsync(command.Territory, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, CRUDMessage.UpdateSuccess);
        }
    }
}
