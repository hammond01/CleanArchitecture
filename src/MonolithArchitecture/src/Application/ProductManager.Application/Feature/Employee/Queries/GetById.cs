using System.Threading;
using System.Threading.Tasks;
using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Employee.Queries;

public record GetEmployeeByIdQuery : IQuery<ApiResponse>
{
    public string EmployeeId { get; set; }

    public GetEmployeeByIdQuery(string employeeId)
    {
        EmployeeId = employeeId;
    }
}

public class GetEmployeeByIdHandler : IQueryHandler<GetEmployeeByIdQuery, ApiResponse>
{
    private readonly ICrudService<ProductManager.Domain.Entities.Employee> _employeeService;

    public GetEmployeeByIdHandler(ICrudService<ProductManager.Domain.Entities.Employee> employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<ApiResponse> HandleAsync(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(request.EmployeeId);
        return employee == null ? new ApiResponse(404, "No data found.")
            : new ApiResponse(200, "Get data by Id successfully", employee);
    }
}
