using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Employee.Commands;
using ProductManager.Application.Feature.Employee.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.EmployeeDto;
namespace ProductManager.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employees")]
public class EmployeeController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public EmployeeController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    [HttpGet]
    [LogAction("Get all employees")]
    public async Task<ActionResult<ApiResponse>> GetEmployees()
    {
        var data = await _dispatcher.DispatchAsync(new GetEmployees());
        data.Result = ((List<Employees>)data.Result).Adapt<List<GetEmployeeDto>>();
        return Ok(data);
    }

    [HttpGet("{id}")]
    [LogAction("Get employee by ID")]
    public async Task<ActionResult<ApiResponse>> GetEmployee(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetEmployeeByIdQuery(id));
        data.Result = ((Employees)data.Result).Adapt<GetEmployeeDto>();
        return Ok(data);
    }

    [HttpPost]
    [LogAction("Create new employee")]
    public async Task<ActionResult<ApiResponse>> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
    {
        var data = createEmployeeDto.Adapt<Employees>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateEmployeeCommand(data));
        var createdEmployee = (Employees)result.Result;
        return Created($"/api/v1.0/employees/{createdEmployee.Id}", result);
    }

    [HttpPut("{id}")]
    [LogAction("Update employee")]
    public async Task<ActionResult<ApiResponse>> UpdateEmployee(string id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetEmployeeByIdQuery(id));
        if (apiResponse.Result is not Employees employee)
        {
            return NotFound(new ApiResponse(404, "Employee not found"));
        }
        updateEmployeeDto.Adapt(employee);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateEmployeeCommand(employee));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [LogAction("Delete employee")]
    public async Task<ActionResult<ApiResponse>> DeleteEmployee(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetEmployeeByIdQuery(id));
        if (apiResponse.Result is not Employees employee)
        {
            return NotFound(new ApiResponse(404, "Employee not found"));
        }
        await _dispatcher.DispatchAsync(new DeleteEmployeeCommand(employee));
        return NoContent();
    }
}
