using Mapster;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Common;
using ProductManager.Application.Feature.Orders.Command;
using ProductManager.Application.Feature.Orders.Queries;
using ProductManager.Domain.Common;
using ProductManager.Domain.Entities;
using ProductManager.Shared.DTOs.OrderDto;
namespace ProductManager.Api.Controllers;

public class OrderController : ConBase
{
    private readonly Dispatcher _dispatcher;

    public OrderController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    public async Task<ApiResponse> GetOrders()
    {
        var data = await _dispatcher.DispatchAsync(new GetOrders());
        data.Result = ((List<Order>)data.Result).Adapt<List<GetOrderDto>>();
        return data;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse> GetOrder(string id)
    {
        var data = await _dispatcher.DispatchAsync(new GetOrderByIdQuery(id));
        data.Result = ((Order)data.Result).Adapt<GetOrderDto>();
        return data;
    }

    [HttpPost]
    public async Task<ApiResponse> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        var data = createOrderDto.Adapt<Order>();
        return await _dispatcher.DispatchAsync(new AddOrUpdateOrderCommand(data));
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> UpdateOrder(string id, [FromBody] UpdateOrderDto updateOrderDto)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetOrderByIdQuery(id));
        var order = (Order)apiResponse.Result;
        updateOrderDto.Adapt(order);
        return await _dispatcher.DispatchAsync(new AddOrUpdateOrderCommand(order));
    }
    [HttpDelete("{id}")]
    public async Task<ApiResponse> DeleteOrder(string id)
    {
        var apiResponse = await _dispatcher.DispatchAsync(new GetOrderByIdQuery(id));
        var order = (Order)apiResponse.Result;
        return await _dispatcher.DispatchAsync(new DeleteOrderCommand(order));
    }
}
