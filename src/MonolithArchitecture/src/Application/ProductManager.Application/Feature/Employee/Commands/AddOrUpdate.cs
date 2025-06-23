using System.Data;
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
    public AddOrUpdateEmployeeCommand(Employees employees)
    {
        Employees = employees;
    }
    public Employees Employees { get; set; }
}
internal class AddOrUpdateEmployeeHandler : ICommandHandler<AddOrUpdateEmployeeCommand, ApiResponse>
{
    private readonly ICrudService<Employees> _employeeService;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrUpdateEmployeeHandler(ICrudService<Employees> employeeService, IUnitOfWork unitOfWork)
    {
        _employeeService = employeeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> HandleAsync(AddOrUpdateEmployeeCommand command, CancellationToken cancellationToken = default)
    {
        using (await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken))
        {
            if (command.Employees.Id == null!)
            {
                command.Employees.Id = UlidExtension.Generate();
                await _employeeService.AddAsync(command.Employees, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new ApiResponse(201, CRUDMessage.CreateSuccess);
            }
            await _employeeService.UpdateAsync(command.Employees, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return new ApiResponse(200, CRUDMessage.UpdateSuccess);
        }
    }
}
