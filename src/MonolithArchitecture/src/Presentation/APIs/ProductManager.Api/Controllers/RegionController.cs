using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Region.Commands;
using ProductManager.Application.Feature.Region.Queries;
using ProductManager.Application.Feature.Region.DTOs;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;

namespace ProductManager.Api.Controllers;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/regions")]
public class RegionController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public RegionController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [LogAction("Get all regions")]
    public async Task<ActionResult<ApiResponse>> GetRegions()
    {
        var data = await _dispatcher.DispatchAsync(new GetRegions());
        data.Result = data.Result.Adapt<List<RegionDto>>();
        return Ok(data);
    }

    [HttpGet("{id}")]
    [LogAction("Get region by ID")]
    public async Task<ActionResult<ApiResponse>> GetRegion(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetRegionByIdQuery(id));
        data.Result = data.Result.Adapt<RegionDto>();
        return Ok(data);
    }

    [HttpPost]
    [LogAction("Create new region")]
    public async Task<ActionResult<ApiResponse>> CreateRegion([FromBody] CreateRegionRequest createRegionRequest)
    {
        var data = createRegionRequest.Adapt<Regions>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateRegionCommand(data));
        var createdRegion = (Regions)result.Result;
        return Created($"/api/v1.0/regions/{createdRegion.Id}", result);
    }

    [HttpPut("{id}")]
    [LogAction("Update region")]
    public async Task<ActionResult<ApiResponse>> UpdateRegion(string id, [FromBody] UpdateRegionRequest updateRegionRequest)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetRegionByIdQuery(id));
        if (apiResponse.Result is not Regions region)
        {
            return NotFound(new ApiResponse(404, "Region not found"));
        }
        updateRegionRequest.Adapt(region);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateRegionCommand(region));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [LogAction("Delete region")]
    public async Task<ActionResult<ApiResponse>> DeleteRegion(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetRegionByIdQuery(id));
        if (apiResponse.Result is not Regions region)
        {
            return NotFound(new ApiResponse(404, "Region not found"));
        }
        await _dispatcher.DispatchAsync(new DeleteRegionCommand(region));
        return NoContent();
    }
}
