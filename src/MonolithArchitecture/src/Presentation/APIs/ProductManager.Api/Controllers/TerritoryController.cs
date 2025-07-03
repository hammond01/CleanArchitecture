using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Territory.Commands;
using ProductManager.Application.Feature.Territory.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.TerritoryDto;

namespace ProductManager.Api.Controllers;

/// <summary>
///     Territory management endpoints with full CRUD operations
/// </summary>
[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/territories")]
public class TerritoryController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public TerritoryController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [LogAction("Get all territories")]
    public async Task<ActionResult<ApiResponse>> GetTerritories()
    {
        var data = await _dispatcher.DispatchAsync(new GetTerritories());
        data.Result = data.Result.Adapt<List<GetTerritoryDto>>();
        return Ok(data);
    }

    [HttpGet("{id}")]
    [LogAction("Get territory by ID")]
    public async Task<ActionResult<ApiResponse>> GetTerritory(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetTerritoryByIdQuery(id));
        data.Result = data.Result.Adapt<GetTerritoryDto>();
        return Ok(data);
    }

    [HttpPost]
    [LogAction("Create new territory")]
    public async Task<ActionResult<ApiResponse>> CreateTerritory([FromBody] CreateTerritoryDto createTerritoryDto)
    {
        var data = createTerritoryDto.Adapt<Territories>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateTerritoryCommand(data));
        var createdTerritory = (Territories)result.Result;
        return Created($"/api/v1.0/territories/{createdTerritory.Id}", result);
    }

    [HttpPut("{id}")]
    [LogAction("Update territory")]
    public async Task<ActionResult<ApiResponse>> UpdateTerritory(string id, [FromBody] UpdateTerritoryDto updateTerritoryDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetTerritoryByIdQuery(id));
        if (apiResponse.Result is not Territories territory)
        {
            return NotFound(new ApiResponse(404, "Territory not found"));
        }
        updateTerritoryDto.Adapt(territory);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateTerritoryCommand(territory));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [LogAction("Delete territory")]
    public async Task<ActionResult<ApiResponse>> DeleteTerritory(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetTerritoryByIdQuery(id));
        if (apiResponse.Result is not Territories territory)
        {
            return NotFound(new ApiResponse(404, "Territory not found"));
        }
        await _dispatcher.DispatchAsync(new DeleteTerritoryCommand(territory));
        return NoContent();
    }
}
