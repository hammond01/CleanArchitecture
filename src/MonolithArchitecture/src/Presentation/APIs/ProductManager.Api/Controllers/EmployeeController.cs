using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Employee.Commands;
using ProductManager.Application.Feature.Employee.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.EmployeeDto;
namespace ProductManager.Api.Controllers;

/// <summary>
///     Employee management controller - provides CRUD operations for employees
/// </summary>
public class EmployeeController : ConBase
{
    private readonly Dispatcher _dispatcher;

    public EmployeeController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    /// <summary>
    ///     Get all employees
    /// </summary>
    /// <returns>List of employees</returns>
    [HttpGet]
    public async Task<ApiResponse> Get()
    {
        var data = await _dispatcher.DispatchAsync(new GetEmployees());
        data.Result = ((List<Employees>)data.Result).Adapt<List<GetEmployeeDto>>();
        return data;
    }
    /// <summary>
    ///     Get employee by ID
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <returns>Employee details</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse> Get(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetEmployeeByIdQuery(id));
        data.Result = ((Employees)data.Result).Adapt<GetEmployeeDto>();
        return data;
    }
    /// <summary>
    ///     Create a new employee
    /// </summary>
    /// <param name="createEmployeeDto">Employee data</param>
    /// <returns>Created employee</returns>
    [HttpPost]
    public async Task<ApiResponse> Post([FromBody] CreateEmployeeDto createEmployeeDto)
    {
        var data = createEmployeeDto.Adapt<Employees>();
        return await _dispatcher.DispatchAsync(new AddOrUpdateEmployeeCommand(data));
    }
    /// <summary>
    ///     Update an existing employee
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <param name="updateEmployeeDto">Updated employee data</param>
    /// <returns>Updated employee</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse> Put(string id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetEmployeeByIdQuery(id));
        var employee = (Employees)apiResponse.Result;
        updateEmployeeDto.Adapt(employee);
        return await _dispatcher.DispatchAsync(new AddOrUpdateEmployeeCommand(employee));
    }
    /// <summary>
    ///     Delete an employee
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <returns>Delete result</returns>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetEmployeeByIdQuery(id));
        var employee = (Employees)apiResponse.Result;
        return await _dispatcher.DispatchAsync(new DeleteEmployeeCommand(employee));
    }
}
