using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Supplier.Commands;
using ProductManager.Application.Feature.Supplier.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.SupplierDto;
namespace ProductManager.Api.Controllers;

[Route("api/v{version:apiVersion}/suppliers")]
[ApiController]
[ApiVersion("1.0")]
public class SupplierController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public SupplierController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    [HttpGet]
    [LogAction("Get all suppliers")]
    public async Task<ActionResult<ApiResponse>> GetSuppliers()
    {
        var data = await _dispatcher.DispatchAsync(new GetSuppliers());
        data.Result = ((List<Suppliers>)data.Result).Adapt<List<GetSupplierDto>>();
        return Ok(data);
    }

    [HttpGet("{id}")]
    [LogAction("Get supplier by ID")]
    public async Task<ActionResult<ApiResponse>> GetSupplier(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetSupplierByIdQuery(id));
        data.Result = ((Suppliers)data.Result).Adapt<GetSupplierDto>();
        return Ok(data);
    }

    [HttpPost]
    [LogAction("Create new supplier")]
    public async Task<ActionResult<ApiResponse>> CreateSupplier([FromBody] CreateSupplierDto createSupplierDto)
    {
        var data = createSupplierDto.Adapt<Suppliers>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateSupplierCommand(data));
        var createdSupplier = (Suppliers)result.Result;
        return Created($"/api/v1.0/suppliers/{createdSupplier.Id}", result);
    }

    [HttpPut("{id}")]
    [LogAction("Update supplier")]
    public async Task<ActionResult<ApiResponse>> UpdateSupplier(string id, [FromBody] UpdateSupplierDto updateSupplierDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetSupplierByIdQuery(id));
        if (apiResponse.Result is not Suppliers supplier)
        {
            return NotFound(new ApiResponse(404, "Supplier not found"));
        }
        updateSupplierDto.Adapt(supplier);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateSupplierCommand(supplier));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [LogAction("Delete supplier")]
    public async Task<ActionResult<ApiResponse>> DeleteSupplier(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetSupplierByIdQuery(id));
        if (apiResponse.Result is not Suppliers supplier)
        {
            return NotFound(new ApiResponse(404, "Supplier not found"));
        }
        await _dispatcher.DispatchAsync(new DeleteSupplierCommand(supplier));
        return NoContent();
    }
}
