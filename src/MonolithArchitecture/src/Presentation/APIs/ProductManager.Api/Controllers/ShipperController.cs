using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Shipper.Commands;
using ProductManager.Application.Feature.Shipper.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Infrastructure.Middleware;
using ProductManager.Shared.DTOs.ShipperDto;
namespace ProductManager.Api.Controllers;

[Route("api/v{version:apiVersion}/shippers")]
[ApiVersion("1.0")]
[ApiController]
public class ShipperController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public ShipperController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    ///     Get all shippers
    /// </summary>
    /// <returns>List of all shippers</returns>
    [HttpGet]
    [LogAction("Get all shippers")]
    public async Task<ActionResult<ApiResponse>> GetShippers()
    {
        var data = await _dispatcher.DispatchAsync(new GetShippers());
        data.Result = data.Result.Adapt<List<GetShipperDto>>();
        return Ok(data);
    }

    /// <summary>
    ///     Get shipper by ID
    /// </summary>
    /// <param name="id">Shipper ID</param>
    /// <returns>Shipper details</returns>
    [HttpGet("{id}")]
    [LogAction("Get shipper by ID")]
    public async Task<ActionResult<ApiResponse>> GetShipper(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetShipperByIdQuery
        {
            ShipperId = id
        });
        data.Result = data.Result.Adapt<GetShipperDto>();
        return Ok(data);
    }

    /// <summary>
    ///     Create a new shipper
    /// </summary>
    /// <param name="createShipperDto">Shipper creation data</param>
    /// <returns>Created shipper</returns>
    [HttpPost]
    [LogAction("Create new shipper")]
    public async Task<ActionResult<ApiResponse>> CreateShipper([FromBody] CreateShipperDto createShipperDto)
    {
        var data = createShipperDto.Adapt<Shippers>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateShipperCommand(data));
        var createdShipper = (Shippers)result.Result;
        return Created($"/api/v1.0/shippers/{createdShipper.Id}", result);
    }

    /// <summary>
    ///     Update an existing shipper
    /// </summary>
    /// <param name="id">Shipper ID</param>
    /// <param name="updateShipperDto">Shipper update data</param>
    /// <returns>Updated shipper</returns>
    [HttpPut("{id}")]
    [EntityLock("Shipper", "{id}", 30)]
    [LogAction("Update shipper")]
    public async Task<ActionResult<ApiResponse>> UpdateShipper(string id, [FromBody] UpdateShipperDto updateShipperDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetShipperByIdQuery
        {
            ShipperId = id
        });
        if (apiResponse.Result is not Shippers shipper)
        {
            return NotFound(new ApiResponse(404, "Shipper not found"));
        }
        updateShipperDto.Adapt(shipper);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateShipperCommand(shipper));
        return Ok(result);
    }

    /// <summary>
    ///     Delete a shipper
    /// </summary>
    /// <param name="id">Shipper ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [LogAction("Delete shipper")]
    public async Task<ActionResult<ApiResponse>> DeleteShipper(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetShipperByIdQuery
        {
            ShipperId = id
        });
        if (apiResponse.Result is not Shippers shipper)
        {
            return NotFound(new ApiResponse(404, "Shipper not found"));
        }
        await _dispatcher.DispatchAsync(new DeleteShipperCommand(shipper));
        return NoContent();
    }
}
