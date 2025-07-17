using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly OrderDbContext _context;

    public HealthController(OrderDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get health status")]
    [SwaggerResponse(200, "Service is healthy")]
    [SwaggerResponse(503, "Service is unhealthy")]
    public async Task<IActionResult> GetHealthStatus()
    {
        try
        {
            // Check database connection
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    service = "OrderManagement API",
                    timestamp = DateTime.UtcNow,
                    checks = new
                    {
                        database = "Failed"
                    }
                });
            }

            // Check database counts
            var orderCount = await _context.Orders.CountAsync();
            var customerCount = await _context.Customers.CountAsync();

            return Ok(new
            {
                status = "Healthy",
                service = "OrderManagement API",
                version = "1.0.0",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                checks = new
                {
                    database = "Healthy",
                    orders = orderCount,
                    customers = customerCount
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                status = "Unhealthy",
                service = "OrderManagement API",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }
}
