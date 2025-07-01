using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Common;
using ProductManager.Persistence;

namespace ProductManager.Api.Controllers;

/// <summary>
/// Controller for global search and filtering across multiple entities
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/search")]
public class SearchController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ApplicationDbContext context, ILogger<SearchController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Global search across all entities
    /// </summary>
    [HttpGet("global")]
    [LogAction("Global search")]
    public async Task<ActionResult<object>> GlobalSearch([FromQuery] string query, [FromQuery] int limit = 20)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return BadRequest("Search query must be at least 2 characters long");
        }

        _logger.LogInformation("🔍 Global search for: {Query}", query);

        var lowerQuery = query.ToLower();

        // Search Products
        var products = await _context.Products
            .Where(p => p.ProductName.ToLower().Contains(lowerQuery))
            .Take(limit / 4)
            .Select(p => new
            {
                p.Id,
                p.ProductName,
                p.UnitPrice,
                p.UnitsInStock,
                p.CategoryId,
                p.SupplierId
            })
            .ToListAsync();

        // Search Customers
        var customers = await _context.Customers
            .Where(c => c.CompanyName.ToLower().Contains(lowerQuery) ||
                       c.ContactName!.ToLower().Contains(lowerQuery))
            .Take(limit / 4)
            .Select(c => new
            {
                c.Id,
                c.CompanyName,
                c.ContactName,
                c.ContactTitle,
                c.City,
                c.Country
            })
            .ToListAsync();

        // Search Employees
        var employees = await _context.Employees
            .Where(e => e.FirstName.ToLower().Contains(lowerQuery) ||
                       e.LastName.ToLower().Contains(lowerQuery))
            .Take(limit / 4)
            .Select(e => new
            {
                e.Id,
                e.FirstName,
                e.LastName,
                e.Title,
                e.City,
                e.Country
            })
            .ToListAsync();

        // Search Categories
        var categories = await _context.Categories
            .Where(cat => cat.CategoryName.ToLower().Contains(lowerQuery))
            .Take(limit / 4)
            .Select(cat => new
            {
                cat.Id,
                cat.CategoryName,
                cat.Description
            })
            .ToListAsync();

        var results = new
        {
            Query = query,
            TotalResults = products.Count + customers.Count + employees.Count + categories.Count,
            Results = new
            {
                Products = products,
                Customers = customers,
                Employees = employees,
                Categories = categories
            }
        };

        _logger.LogInformation("✅ Global search completed. Found {Count} results", results.TotalResults);
        return Ok(results);
    }

    /// <summary>
    /// Advanced product search with multiple filters
    /// </summary>
    [HttpGet("products/advanced")]
    [LogAction("Advanced product search")]
    public async Task<ActionResult<object>> AdvancedProductSearch(
        [FromQuery] string? name = null,
        [FromQuery] string? categoryId = null,
        [FromQuery] string? supplierId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int? minStock = null,
        [FromQuery] bool? inStock = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("🔍 Advanced product search with filters");

        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.ProductName.ToLower().Contains(name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(supplierId))
        {
            query = query.Where(p => p.SupplierId == supplierId);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice <= maxPrice.Value);
        }

        if (minStock.HasValue)
        {
            query = query.Where(p => p.UnitsInStock >= minStock.Value);
        }

        if (inStock.HasValue)
        {
            query = inStock.Value
                ? query.Where(p => p.UnitsInStock > 0)
                : query.Where(p => p.UnitsInStock == 0);
        }

        var totalCount = await query.CountAsync();
        var products = await query
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .OrderBy(p => p.ProductName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.ProductName,
                p.UnitPrice,
                p.UnitsInStock,
                p.CategoryId,
                p.SupplierId,
                p.Category!.CategoryName,
                SupplierName = p.Supplier!.CompanyName
            })
            .ToListAsync();

        var result = new
        {
            Data = products,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            Filters = new
            {
                Name = name,
                CategoryId = categoryId,
                SupplierId = supplierId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinStock = minStock,
                InStock = inStock
            }
        };

        _logger.LogInformation("✅ Advanced product search completed. Found {Count} results", totalCount);
        return Ok(result);
    }

    /// <summary>
    /// Search customers with filters
    /// </summary>
    [HttpGet("customers")]
    [LogAction("Customer search")]
    public async Task<ActionResult<object>> SearchCustomers(
        [FromQuery] string? query = null,
        [FromQuery] string? city = null,
        [FromQuery] string? country = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("🔍 Customer search with query: {Query}", query);

        var customerQuery = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            customerQuery = customerQuery.Where(c =>
                c.CompanyName.ToLower().Contains(lowerQuery) ||
                c.ContactName!.ToLower().Contains(lowerQuery) ||
                c.ContactTitle!.ToLower().Contains(lowerQuery));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            customerQuery = customerQuery.Where(c => c.City!.ToLower().Contains(city.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            customerQuery = customerQuery.Where(c => c.Country!.ToLower().Contains(country.ToLower()));
        }

        var totalCount = await customerQuery.CountAsync();
        var customers = await customerQuery
            .OrderBy(c => c.CompanyName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.CompanyName,
                c.ContactName,
                c.ContactTitle,
                c.Address,
                c.City,
                c.Region,
                c.PostalCode,
                c.Country,
                c.Phone,
                c.Fax
            })
            .ToListAsync();

        var result = new
        {
            Data = customers,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            Filters = new
            {
                Query = query,
                City = city,
                Country = country
            }
        };

        _logger.LogInformation("✅ Customer search completed. Found {Count} results", totalCount);
        return Ok(result);
    }
}
