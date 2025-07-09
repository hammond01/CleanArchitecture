using System.ComponentModel.DataAnnotations;

namespace ProductManager.Shared.DTOs.CategoryDto;

/// <summary>
/// Data Transfer Object for creating a new category
/// </summary>
/// <remarks>
/// This DTO contains the required information to create a new category in the system.
/// All properties are validated according to business rules and database constraints.
/// </remarks>
public class CreateCategoryDto
{
    /// <summary>
    /// Gets or sets the name of the category
    /// </summary>
    /// <value>
    /// The unique name that identifies the category. Must be between 1 and 150 characters.
    /// </value>
    /// <example>Electronics</example>
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(150, MinimumLength = 1, ErrorMessage = "Category name must be between 1 and 150 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-&().,]+$", ErrorMessage = "Category name contains invalid characters")]
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the category
    /// </summary>
    /// <value>
    /// An optional description providing additional details about the category.
    /// Maximum length is 250 characters.
    /// </value>
    /// <example>Consumer electronics including phones, laptops, and accessories</example>
    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
    public string? Description { get; set; }
}
