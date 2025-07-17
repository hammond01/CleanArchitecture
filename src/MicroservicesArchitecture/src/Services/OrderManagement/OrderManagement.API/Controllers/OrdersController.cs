using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.DTOs;
using OrderManagement.API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all orders with pagination")]
    [SwaggerResponse(200, "Returns list of orders", typeof(IEnumerable<OrderSummaryDto>))]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var orders = await _orderService.GetOrdersAsync(pageNumber, pageSize);
        return Ok(orders);
    }

    [HttpGet("{orderId:int}")]
    [SwaggerOperation(Summary = "Get order by ID")]
    [SwaggerResponse(200, "Returns order details", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    public async Task<ActionResult<OrderDto>> GetOrder(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        
        if (order == null)
            return NotFound($"Order with ID {orderId} not found");

        return Ok(order);
    }

    [HttpGet("by-number/{orderNumber}")]
    [SwaggerOperation(Summary = "Get order by order number")]
    [SwaggerResponse(200, "Returns order details", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    public async Task<ActionResult<OrderDto>> GetOrderByNumber(string orderNumber)
    {
        var order = await _orderService.GetOrderByNumberAsync(orderNumber);
        
        if (order == null)
            return NotFound($"Order with number {orderNumber} not found");

        return Ok(order);
    }

    [HttpGet("customer/{customerId:int}")]
    [SwaggerOperation(Summary = "Get orders by customer ID")]
    [SwaggerResponse(200, "Returns customer orders", typeof(IEnumerable<OrderSummaryDto>))]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetOrdersByCustomer(int customerId)
    {
        var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
        return Ok(orders);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new order")]
    [SwaggerResponse(201, "Order created successfully", typeof(OrderDto))]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(createOrderDto);
            return CreatedAtAction(nameof(GetOrder), new { orderId = order.OrderId }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{orderId:int}")]
    [SwaggerOperation(Summary = "Update an existing order")]
    [SwaggerResponse(200, "Order updated successfully", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<ActionResult<OrderDto>> UpdateOrder(int orderId, [FromBody] UpdateOrderDto updateOrderDto)
    {
        var order = await _orderService.UpdateOrderAsync(orderId, updateOrderDto);
        
        if (order == null)
            return NotFound($"Order with ID {orderId} not found");

        return Ok(order);
    }

    [HttpPatch("{orderId:int}/status")]
    [SwaggerOperation(Summary = "Update order status")]
    [SwaggerResponse(200, "Order status updated successfully", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    [SwaggerResponse(400, "Invalid status")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto updateStatusDto)
    {
        var order = await _orderService.UpdateOrderStatusAsync(orderId, updateStatusDto);
        
        if (order == null)
            return NotFound($"Order with ID {orderId} not found");

        return Ok(order);
    }

    [HttpPost("{orderId:int}/cancel")]
    [SwaggerOperation(Summary = "Cancel an order")]
    [SwaggerResponse(200, "Order cancelled successfully")]
    [SwaggerResponse(404, "Order not found")]
    [SwaggerResponse(400, "Order cannot be cancelled")]
    public async Task<ActionResult> CancelOrder(int orderId)
    {
        var success = await _orderService.CancelOrderAsync(orderId);
        
        if (!success)
            return BadRequest("Order cannot be cancelled or does not exist");

        return Ok(new { message = "Order cancelled successfully" });
    }

    [HttpDelete("{orderId:int}")]
    [SwaggerOperation(Summary = "Delete an order")]
    [SwaggerResponse(200, "Order deleted successfully")]
    [SwaggerResponse(404, "Order not found")]
    [SwaggerResponse(400, "Order cannot be deleted")]
    public async Task<ActionResult> DeleteOrder(int orderId)
    {
        var success = await _orderService.DeleteOrderAsync(orderId);
        
        if (!success)
            return BadRequest("Order cannot be deleted or does not exist");

        return Ok(new { message = "Order deleted successfully" });
    }
}
