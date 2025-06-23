using System.Threading;
using System.Threading.Tasks;
using ProductManager.Application.Common.Commands;
using ProductManager.Application.Common.Services;
using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Employee.Commands;

public class DeleteEmployeeCommand : ICommand<ApiResponse>
{
    public ProductManager.Domain.Entities.Employee Employee { get; set; }

    public DeleteEmployeeCommand(ProductManager.Domain.Entities.Employee employee)
    {
        Employee = employee;
    }
}

public class DeleteEmployeeCommandHandler : ICommandHandler<DeleteEmployeeCommand, ApiResponse>
{
    private readonly ICrudService<ProductManager.Domain.Entities.Employee> _employeeService;

    public DeleteEmployeeCommandHandler(ICrudService<ProductManager.Domain.Entities.Employee> employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<ApiResponse> HandleAsync(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        await _employeeService.DeleteAsync(request.Employee, cancellationToken);

        return new ApiResponse
        {
            StatusCode = 200,
            Message = CRUDMessage.DeleteSuccess,
            Result = null!
        };
    }
}
