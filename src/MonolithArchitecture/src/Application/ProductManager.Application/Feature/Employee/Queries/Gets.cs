using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProductManager.Application.Common.Queries;
using ProductManager.Application.Common.Services;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Feature.Employee.Queries;

public record GetEmployees : IQuery<ApiResponse>;

internal class GetEmployeesHandler : IQueryHandler<GetEmployees, ApiResponse>
{
    private readonly ICrudService<ProductManager.Domain.Entities.Employee> _employeeService;

    public GetEmployeesHandler(ICrudService<ProductManager.Domain.Entities.Employee> employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<ApiResponse> HandleAsync(GetEmployees request, CancellationToken cancellationToken = default)
    {
        var response = await _employeeService.GetAsync();
        return new ApiResponse(200, "Get employees successfully", response);
    }
}
