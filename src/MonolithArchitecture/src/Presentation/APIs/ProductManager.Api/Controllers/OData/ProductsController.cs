using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ProductManager.Domain.Entities;
using ProductManager.Persistence;

namespace ProductManager.Api.Controllers.OData;

/// <summary>
/// OData controller for Products with advanced query capabilities
/// </summary>
public class ProductsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Get products with OData query support
    /// </summary>
    /// <returns>Queryable collection of products</returns>
    /// <response code="200">Returns the products</response>
    /// <response code="400">If the query is invalid</response>
    [HttpGet]
    [EnableQuery(PageSize = 50, MaxTop = 1000)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IQueryable<Products> Get()
    {
        return _context.Products;
    }

    /// <summary>
    /// Get a specific product by ID with OData query support
    /// </summary>
    /// <param name="key">Product ID</param>
    /// <returns>Product entity</returns>
    /// <response code="200">Returns the product</response>
    /// <response code="404">If the product is not found</response>
    [HttpGet]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromRoute] string key)
    {
        var product = _context.Products.Find(key);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }
}
