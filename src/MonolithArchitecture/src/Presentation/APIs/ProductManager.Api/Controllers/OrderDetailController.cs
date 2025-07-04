using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.OrderDetail.Commands;
using ProductManager.Application.Feature.OrderDetail.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Infrastructure.Middleware;
using ProductManager.Shared.DTOs.OrderDetailDto;

namespace ProductManager.Api.Controllers;

/// <summary>
///     OrderDetail management endpoints with full CRUD operations
/// </summary>
[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orderdetails")]
public class OrderDetailController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public OrderDetailController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [LogAction("Get all order details")]
    public async Task<ActionResult<ApiResponse>> GetOrderDetails()
    {
        var data = await _dispatcher.DispatchAsync(new GetOrderDetails());
        data.Result = data.Result.Adapt<List<GetOrderDetailDto>>();
        return Ok(data);
    }

    [HttpGet("{id}")]
    [LogAction("Get order detail by ID")]
    public async Task<ActionResult<ApiResponse>> GetOrderDetail(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetOrderDetailByIdQuery(id));
        data.Result = data.Result.Adapt<GetOrderDetailDto>();
        return Ok(data);
    }

    [HttpPost]
    [LogAction("Create new order detail")]
    public async Task<ActionResult<ApiResponse>> CreateOrderDetail([FromBody] CreateOrderDetailDto createOrderDetailDto)
    {
        var data = createOrderDetailDto.Adapt<OrderDetails>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateOrderDetailCommand(data));
        var createdOrderDetail = (OrderDetails)result.Result;
        return Created($"/api/v1.0/orderdetails/{createdOrderDetail.Id}", result);
    }

    [HttpPut("{id}")]
    [LogAction("Update order detail")]
    public async Task<ActionResult<ApiResponse>> UpdateOrderDetail(string id, [FromBody] UpdateOrderDetailDto updateOrderDetailDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetOrderDetailByIdQuery(id));
        if (apiResponse.Result is not OrderDetails orderDetail)
        {
            return NotFound(new ApiResponse(404, "Order detail not found"));
        }
        updateOrderDetailDto.Adapt(orderDetail);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateOrderDetailCommand(orderDetail));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [LogAction("Delete order detail")]
    public async Task<ActionResult<ApiResponse>> DeleteOrderDetail(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetOrderDetailByIdQuery(id));
        if (apiResponse.Result is not OrderDetails orderDetail)
        {
            return NotFound(new ApiResponse(404, "Order detail not found"));
        }
        await _dispatcher.DispatchAsync(new DeleteOrderDetailCommand(orderDetail));
        return NoContent();
    }
}
