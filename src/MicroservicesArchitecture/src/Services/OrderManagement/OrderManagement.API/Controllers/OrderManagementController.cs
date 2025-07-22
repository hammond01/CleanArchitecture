using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Commands;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Queries;
using OrderManagement.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderManagement.API.Controllers;

/// <summary>
/// Order Management Controller using Clean Architecture with CQRS
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class OrderManagementController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderManagementController> _logger;

    public OrderManagementController(IMediator mediator, ILogger<OrderManagementController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all orders with pagination")]
    [SwaggerResponse(200, "Returns paged list of orders", typeof(PagedResultDto<OrderSummaryDto>))]
    [SwaggerResponse(400, "Invalid request parameters")]
    public async Task<ActionResult<PagedResultDto<OrderSummaryDto>>> GetOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null,
        [FromQuery] string? customerId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var query = new GetOrdersQuery(pageNumber, pageSize, status, customerId, startDate, endDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders");
            return BadRequest("Error retrieving orders");
        }
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{orderId}")]
    [SwaggerOperation(Summary = "Get order by ID")]
    [SwaggerResponse(200, "Returns order details", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    public async Task<ActionResult<OrderDto>> GetOrder(string orderId)
    {
        try
        {
            var query = new GetOrderByIdQuery(orderId);
            var result = await _mediator.Send(query);
            
            if (result == null)
            {
                return NotFound($"Order with ID {orderId} not found");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", orderId);
            return BadRequest("Error retrieving order");
        }
    }

    /// <summary>
    /// Get order by order number
    /// </summary>
    [HttpGet("by-number/{orderNumber}")]
    [SwaggerOperation(Summary = "Get order by order number")]
    [SwaggerResponse(200, "Returns order details", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    public async Task<ActionResult<OrderDto>> GetOrderByNumber(string orderNumber)
    {
        try
        {
            var query = new GetOrderByOrderNumberQuery(orderNumber);
            var result = await _mediator.Send(query);
            
            if (result == null)
            {
                return NotFound($"Order with number {orderNumber} not found");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order by number {OrderNumber}", orderNumber);
            return BadRequest("Error retrieving order");
        }
    }

    /// <summary>
    /// Get orders by customer ID
    /// </summary>
    [HttpGet("customer/{customerId}")]
    [SwaggerOperation(Summary = "Get orders by customer ID")]
    [SwaggerResponse(200, "Returns list of customer orders", typeof(IEnumerable<OrderSummaryDto>))]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetOrdersByCustomer(string customerId)
    {
        try
        {
            var query = new GetOrdersByCustomerIdQuery(customerId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders for customer {CustomerId}", customerId);
            return BadRequest("Error retrieving customer orders");
        }
    }

    /// <summary>
    /// Get orders by status
    /// </summary>
    [HttpGet("status/{status}")]
    [SwaggerOperation(Summary = "Get orders by status")]
    [SwaggerResponse(200, "Returns list of orders with specified status", typeof(IEnumerable<OrderSummaryDto>))]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetOrdersByStatus(OrderStatus status)
    {
        try
        {
            var query = new GetOrdersByStatusQuery(status);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by status {Status}", status);
            return BadRequest("Error retrieving orders by status");
        }
    }

    /// <summary>
    /// Get order statistics
    /// </summary>
    [HttpGet("statistics")]
    [SwaggerOperation(Summary = "Get order statistics")]
    [SwaggerResponse(200, "Returns order statistics", typeof(OrderStatisticsDto))]
    public async Task<ActionResult<OrderStatisticsDto>> GetOrderStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var query = new GetOrderStatisticsQuery(startDate, endDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order statistics");
            return BadRequest("Error retrieving order statistics");
        }
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new order")]
    [SwaggerResponse(201, "Order created successfully", typeof(OrderDto))]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            var command = new CreateOrderCommand(createOrderDto);
            var result = await _mediator.Send(command);
            
            return CreatedAtAction(
                nameof(GetOrder), 
                new { orderId = result.Id }, 
                result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid order data");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return BadRequest("Error creating order");
        }
    }

    /// <summary>
    /// Update an existing order
    /// </summary>
    [HttpPut("{orderId}")]
    [SwaggerOperation(Summary = "Update an existing order")]
    [SwaggerResponse(200, "Order updated successfully", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<ActionResult<OrderDto>> UpdateOrder(string orderId, [FromBody] UpdateOrderDto updateOrderDto)
    {
        try
        {
            var command = new UpdateOrderCommand(orderId, updateOrderDto);
            var result = await _mediator.Send(command);
            
            if (result == null)
            {
                return NotFound($"Order with ID {orderId} not found");
            }

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid order data");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId}", orderId);
            return BadRequest("Error updating order");
        }
    }

    /// <summary>
    /// Confirm an order
    /// </summary>
    [HttpPost("{orderId}/confirm")]
    [SwaggerOperation(Summary = "Confirm an order")]
    [SwaggerResponse(200, "Order confirmed successfully", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    [SwaggerResponse(400, "Invalid operation")]
    public async Task<ActionResult<OrderDto>> ConfirmOrder(string orderId)
    {
        try
        {
            var command = new ConfirmOrderCommand(orderId);
            var result = await _mediator.Send(command);
            
            if (result == null)
            {
                return NotFound($"Order with ID {orderId} not found");
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming order {OrderId}", orderId);
            return BadRequest("Error confirming order");
        }
    }

    /// <summary>
    /// Cancel an order
    /// </summary>
    [HttpPost("{orderId}/cancel")]
    [SwaggerOperation(Summary = "Cancel an order")]
    [SwaggerResponse(200, "Order cancelled successfully", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    [SwaggerResponse(400, "Invalid operation")]
    public async Task<ActionResult<OrderDto>> CancelOrder(string orderId, [FromBody] string reason)
    {
        try
        {
            var command = new CancelOrderCommand(orderId, reason);
            var result = await _mediator.Send(command);
            
            if (result == null)
            {
                return NotFound($"Order with ID {orderId} not found");
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
            return BadRequest("Error cancelling order");
        }
    }

    /// <summary>
    /// Ship an order
    /// </summary>
    [HttpPost("{orderId}/ship")]
    [SwaggerOperation(Summary = "Ship an order")]
    [SwaggerResponse(200, "Order shipped successfully", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    [SwaggerResponse(400, "Invalid operation")]
    public async Task<ActionResult<OrderDto>> ShipOrder(string orderId, [FromBody] string trackingNumber)
    {
        try
        {
            var command = new ShipOrderCommand(orderId, trackingNumber);
            var result = await _mediator.Send(command);
            
            if (result == null)
            {
                return NotFound($"Order with ID {orderId} not found");
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shipping order {OrderId}", orderId);
            return BadRequest("Error shipping order");
        }
    }

    /// <summary>
    /// Mark order as delivered
    /// </summary>
    [HttpPost("{orderId}/deliver")]
    [SwaggerOperation(Summary = "Mark order as delivered")]
    [SwaggerResponse(200, "Order delivered successfully", typeof(OrderDto))]
    [SwaggerResponse(404, "Order not found")]
    [SwaggerResponse(400, "Invalid operation")]
    public async Task<ActionResult<OrderDto>> DeliverOrder(string orderId)
    {
        try
        {
            var command = new DeliverOrderCommand(orderId);
            var result = await _mediator.Send(command);
            
            if (result == null)
            {
                return NotFound($"Order with ID {orderId} not found");
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delivering order {OrderId}", orderId);
            return BadRequest("Error delivering order");
        }
    }
}
