using System.ComponentModel.DataAnnotations;

namespace ProductManager.Shared.DTOs.CategoryDto;

/// <summary>
/// Data Transfer Object for category filtering and pagination requests
/// </summary>
/// <remarks>
/// This DTO is used for advanced category queries with filtering, sorting, and pagination capabilities.
/// It provides comprehensive options for category data retrieval.
/// </remarks>
public class CategoryFilterDto
{
    /// <summary>
    /// Gets or sets the search term for category name or description
    /// </summary>
    /// <value>
    /// Optional search term to filter categories by name or description.
    /// Case-insensitive search.
    /// </value>
    /// <example>Electronics</example>
    [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the page number for pagination
    /// </summary>
    /// <value>
    /// The page number to retrieve (1-based). Default is 1.
    /// </value>
    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size for pagination
    /// </summary>
    /// <value>
    /// The number of items per page. Default is 10, maximum is 100.
    /// </value>
    /// <example>10</example>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the field to sort by
    /// </summary>
    /// <value>
    /// The field name to sort by. Supported values: name, created, updated.
    /// Default is name.
    /// </value>
    /// <example>name</example>
    [RegularExpression("^(name|created|updated)$", ErrorMessage = "Sort field must be one of: name, created, updated")]
    public string? SortBy { get; set; } = "name";

    /// <summary>
    /// Gets or sets the sort direction
    /// </summary>
    /// <value>
    /// True for descending order, false for ascending. Default is false.
    /// </value>
    /// <example>false</example>
    public bool SortDescending { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to include categories with no products
    /// </summary>
    /// <value>
    /// True to include empty categories, false to exclude them. Default is true.
    /// </value>
    /// <example>true</example>
    public bool IncludeEmpty { get; set; } = true;

    /// <summary>
    /// Gets or sets the start date for filtering by creation date
    /// </summary>
    /// <value>
    /// Optional start date to filter categories created after this date.
    /// </value>
    /// <example>2024-01-01T00:00:00Z</example>
    public DateTimeOffset? CreatedFrom { get; set; }

    /// <summary>
    /// Gets or sets the end date for filtering by creation date
    /// </summary>
    /// <value>
    /// Optional end date to filter categories created before this date.
    /// </value>
    /// <example>2024-12-31T23:59:59Z</example>
    public DateTimeOffset? CreatedTo { get; set; }

    /// <summary>
    /// Gets or sets whether to include product count in the response
    /// </summary>
    /// <value>
    /// True to include product count statistics, false to exclude them. Default is false.
    /// </value>
    /// <example>false</example>
    public bool IncludeProductCount { get; set; } = false;
}

/// <summary>
/// Data Transfer Object for paginated category results
/// </summary>
/// <remarks>
/// This DTO represents a paginated result set of categories with metadata about pagination.
/// </remarks>
public class PagedCategoryDto
{
    /// <summary>
    /// Gets or sets the list of categories for the current page
    /// </summary>
    /// <value>
    /// The collection of categories for the requested page.
    /// </value>
    [Required]
    public IEnumerable<GetCategoryDto> Items { get; set; } = new List<GetCategoryDto>();

    /// <summary>
    /// Gets or sets the total number of categories matching the filter
    /// </summary>
    /// <value>
    /// The total count of categories across all pages.
    /// </value>
    /// <example>150</example>
    [Range(0, int.MaxValue, ErrorMessage = "Total count must be non-negative")]
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the current page number
    /// </summary>
    /// <value>
    /// The current page number (1-based).
    /// </value>
    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    /// <value>
    /// The number of items per page.
    /// </value>
    /// <example>10</example>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; }

    /// <summary>
    /// Gets the total number of pages
    /// </summary>
    /// <value>
    /// The total number of pages based on total count and page size.
    /// </value>
    /// <example>15</example>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets whether there is a previous page
    /// </summary>
    /// <value>
    /// True if there is a previous page, false otherwise.
    /// </value>
    /// <example>false</example>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Gets whether there is a next page
    /// </summary>
    /// <value>
    /// True if there is a next page, false otherwise.
    /// </value>
    /// <example>true</example>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Data Transfer Object for category statistics
/// </summary>
/// <remarks>
/// This DTO contains statistical information about a category including product counts and values.
/// </remarks>
public class CategoryStatisticsDto
{
    /// <summary>
    /// Gets or sets the category identifier
    /// </summary>
    /// <value>
    /// The unique identifier of the category.
    /// </value>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    [Required]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category name
    /// </summary>
    /// <value>
    /// The name of the category.
    /// </value>
    /// <example>Electronics</example>
    [Required]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of products in the category
    /// </summary>
    /// <value>
    /// The total count of products (including discontinued ones).
    /// </value>
    /// <example>25</example>
    [Range(0, int.MaxValue, ErrorMessage = "Product count must be non-negative")]
    public int ProductCount { get; set; }

    /// <summary>
    /// Gets or sets the number of active products in the category
    /// </summary>
    /// <value>
    /// The count of products that are not discontinued.
    /// </value>
    /// <example>20</example>
    [Range(0, int.MaxValue, ErrorMessage = "Active product count must be non-negative")]
    public int ActiveProductCount { get; set; }

    /// <summary>
    /// Gets or sets the total value of all products in the category
    /// </summary>
    /// <value>
    /// The sum of all product prices in the category.
    /// </value>
    /// <example>1250.50</example>
    [Range(0, double.MaxValue, ErrorMessage = "Total value must be non-negative")]
    public decimal TotalValue { get; set; }

    /// <summary>
    /// Gets or sets the average price of products in the category
    /// </summary>
    /// <value>
    /// The average price of products in the category.
    /// </value>
    /// <example>50.02</example>
    [Range(0, double.MaxValue, ErrorMessage = "Average price must be non-negative")]
    public decimal AveragePrice { get; set; }

    /// <summary>
    /// Gets or sets the total units in stock for the category
    /// </summary>
    /// <value>
    /// The sum of units in stock for all products in the category.
    /// </value>
    /// <example>500</example>
    [Range(0, int.MaxValue, ErrorMessage = "Units in stock must be non-negative")]
    public int UnitsInStock { get; set; }

    /// <summary>
    /// Gets or sets the last update date
    /// </summary>
    /// <value>
    /// The date when the category was last updated.
    /// </value>
    /// <example>2024-01-20T14:45:00Z</example>
    [Required]
    public DateTimeOffset LastUpdated { get; set; }
}

/// <summary>
/// Data Transfer Object for category with product count
/// </summary>
/// <remarks>
/// This DTO extends the basic category information with product count statistics.
/// </remarks>
public class CategoryWithProductCountDto : GetCategoryDto
{
    /// <summary>
    /// Gets or sets the total number of products in the category
    /// </summary>
    /// <value>
    /// The total count of products (including discontinued ones).
    /// </value>
    /// <example>25</example>
    [Range(0, int.MaxValue, ErrorMessage = "Product count must be non-negative")]
    public int ProductCount { get; set; }

    /// <summary>
    /// Gets or sets the number of active products in the category
    /// </summary>
    /// <value>
    /// The count of products that are not discontinued.
    /// </value>
    /// <example>20</example>
    [Range(0, int.MaxValue, ErrorMessage = "Active product count must be non-negative")]
    public int ActiveProductCount { get; set; }
}

/// <summary>
/// Data Transfer Object for bulk category operations
/// </summary>
/// <remarks>
/// This DTO is used for bulk operations on multiple categories.
/// </remarks>
public class BulkCategoryOperationDto
{
    /// <summary>
    /// Gets or sets the list of category IDs to operate on
    /// </summary>
    /// <value>
    /// The collection of category identifiers for bulk operations.
    /// </value>
    [Required]
    [MinLength(1, ErrorMessage = "At least one category ID is required")]
    public IEnumerable<string> CategoryIds { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the operation type
    /// </summary>
    /// <value>
    /// The type of bulk operation to perform. Supported values: delete, activate, deactivate.
    /// </value>
    /// <example>delete</example>
    [Required]
    [RegularExpression("^(delete|activate|deactivate)$", ErrorMessage = "Operation must be one of: delete, activate, deactivate")]
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional parameters for the operation
    /// </summary>
    /// <value>
    /// Optional parameters specific to the operation type.
    /// </value>
    public Dictionary<string, object>? Parameters { get; set; }
}
