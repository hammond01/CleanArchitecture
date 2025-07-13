using Microsoft.AspNetCore.Mvc;

namespace ProductCatalog.API.Controllers;

/// <summary>
/// Products controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<object>> GetAllProducts()
    {
        // Mock data for now
        var products = new[]
        {
            new { ProductId = "01JH179GGZ7FAHZ0DNFYNZ20AA", ProductName = "Laptop Dell XPS 13", CategoryId = "01JH179GGZ7FAHZ0DNFYNZ10AA", UnitPrice = 1299.99m, UnitsInStock = 15 },
            new { ProductId = "01JH179GGZ7FAHZ0DNFYNZ22CC", ProductName = "iPhone 15 Pro", CategoryId = "01JH179GGZ7FAHZ0DNFYNZ12CC", UnitPrice = 999.99m, UnitsInStock = 25 }
        };

        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<object> GetProductById(string id)
    {
        // Mock data for now
        var product = new
        {
            ProductId = id,
            ProductName = "Laptop Dell XPS 13",
            CategoryId = "01JH179GGZ7FAHZ0DNFYNZ10AA",
            CategoryName = "Computers",
            SupplierId = "01JH179GGZ7FAHZ0DNFYNZ30AA",
            SupplierName = "Tech Supplies Co.",
            UnitPrice = 1299.99m,
            UnitsInStock = 15,
            Discontinued = false
        };

        return Ok(product);
    }

    /// <summary>
    /// Create new product
    /// </summary>
    [HttpPost]
    public ActionResult<string> CreateProduct([FromBody] object command)
    {
        // Mock implementation
        var productId = "01JH179GGZ7FAHZ0DNFYNZ" + DateTime.UtcNow.Ticks.ToString()[^6..];
        return CreatedAtAction(nameof(GetProductById), new { id = productId }, productId);
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public ActionResult<object> HealthCheck()
    {
        return Ok(new { Status = "Healthy", Service = "ProductCatalog API", Timestamp = DateTime.UtcNow });
    }
}
