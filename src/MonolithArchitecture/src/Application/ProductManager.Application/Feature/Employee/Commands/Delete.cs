using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
namespace ProductManager.Application.Feature.Employee.Commands;

public class DeleteEmployeeCommand : ICommand<ApiResponse>
{

    public DeleteEmployeeCommand(Employees employees)
    {
        Employees = employees;
    }
    public Employees Employees { get; set; }
}
public class DeleteEmployeeCommandHandler : ICommandHandler<DeleteEmployeeCommand, ApiResponse>
{
    private readonly ICrudService<Employees> _employeeService;

    public DeleteEmployeeCommandHandler(ICrudService<Employees> employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<ApiResponse> HandleAsync(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        await _employeeService.DeleteAsync(request.Employees, cancellationToken);

        return new ApiResponse
        {
            StatusCode = 200, Message = CRUDMessage.DeleteSuccess, Result = null!
        };
    }
}
