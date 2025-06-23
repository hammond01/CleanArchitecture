using System.Data;
using System.Threading;
using System.Threading.Tasks;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Persistence.Extensions;

namespace ProductManager.Application.Feature.Employee.Commands;

public class AddOrUpdateEmployeeCommand : ICommand<ApiResponse>
{
    public ProductManager.Domain.Entities.Employee Employee { get; set; }

    public AddOrUpdateEmployeeCommand(ProductManager.Domain.Entities.Employee employee)
    {
        Employee = employee;
    }
}

internal class AddOrUpdateEmployeeHandler : ICommandHandler<AddOrUpdateEmployeeCommand, ApiResponse>
{
    private readonly ICrudService<ProductManager.Domain.Entities.Employee> _employeeService;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrUpdateEmployeeHandler(ICrudService<ProductManager.Domain.Entities.Employee> employeeService, IUnitOfWork unitOfWork)
    {
        _employeeService = employeeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> HandleAsync(AddOrUpdateEmployeeCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            if (command.Employee.Id == null!)
            {
                command.Employee.Id = UlidExtension.Generate();
                await _employeeService.AddAsync(command.Employee, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new ApiResponse(201, CRUDMessage.CreateSuccess);
            }
            await _employeeService.UpdateAsync(command.Employee, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, CRUDMessage.UpdateSuccess);
        }
    }
}
