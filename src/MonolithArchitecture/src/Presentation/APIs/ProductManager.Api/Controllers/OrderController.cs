using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Orders.Command;
using ProductManager.Application.Feature.Orders.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.OrderDto;
namespace ProductManager.Api.Controllers;

[Route("api/v{version:apiVersion}/orders")]
[ApiVersion("1.0")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public OrderController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [LogAction("Get all orders")]
    public async Task<ActionResult<ApiResponse>> GetOrders()
    {
        var data = await _dispatcher.DispatchAsync(new GetOrders());
        data.Result = data.Result.Adapt<List<GetOrderDto>>();
        return Ok(data);
    }

    [HttpGet("{id}")]
    [LogAction("Get order by ID")]
    public async Task<ActionResult<ApiResponse>> GetOrder(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetOrderByIdQuery(id));
        data.Result = data.Result.Adapt<GetOrderDto>();
        return Ok(data);
    }

    [HttpPost]
    [LogAction("Create new order")]
    public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        var data = createOrderDto.Adapt<Order>();
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateOrderCommand(data));
        var createdOrder = (Order)result.Result;
        return Created($"/api/v1.0/orders/{createdOrder.Id}", result);
    }

    [HttpPut("{id}")]
    [LogAction("Update order")]
    public async Task<ActionResult<ApiResponse>> UpdateOrder(string id, [FromBody] UpdateOrderDto updateOrderDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetOrderByIdQuery(id));
        var order = (Order)apiResponse.Result;
        updateOrderDto.Adapt(order);
        var result = await _dispatcher.DispatchAsync(new AddOrUpdateOrderCommand(order));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [LogAction("Delete order")]
    public async Task<ActionResult<ApiResponse>> DeleteOrder(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetOrderByIdQuery(id));
        var order = (Order)apiResponse.Result;
        await _dispatcher.DispatchAsync(new DeleteOrderCommand(order));
        return NoContent();
    }
}
