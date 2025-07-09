using System.ComponentModel.DataAnnotations;

namespace ProductManager.Shared.DTOs.CategoryDto;

/// <summary>
/// Data Transfer Object for category information retrieval
/// </summary>
/// <remarks>
/// This DTO represents the complete category information returned by the API.
/// It includes all category properties along with audit information.
/// </remarks>
public class GetCategoryDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the category
    /// </summary>
    /// <value>
    /// The unique identifier that distinguishes this category from others.
    /// </value>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    [Required]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the category
    /// </summary>
    /// <value>
    /// The name that identifies the category. Maximum length is 150 characters.
    /// </value>
    /// <example>Electronics</example>
    [Required]
    [StringLength(150)]
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the category
    /// </summary>
    /// <value>
    /// An optional description providing additional details about the category.
    /// Maximum length is 250 characters.
    /// </value>
    /// <example>Consumer electronics including phones, laptops, and accessories</example>
    [StringLength(250)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the category picture as byte array
    /// </summary>
    /// <value>
    /// An optional binary representation of the category picture.
    /// </value>
    /// <remarks>
    /// This property contains the raw image data. For API responses, consider using PictureLink instead
    /// to avoid large payload sizes.
    /// </remarks>
    public byte[]? Picture { get; set; }

    /// <summary>
    /// Gets or sets the URL link to the category picture
    /// </summary>
    /// <value>
    /// An optional URL pointing to the category picture resource.
    /// Maximum length is 100 characters.
    /// </value>
    /// <example>https://example.com/images/categories/electronics.jpg</example>
    [StringLength(100)]
    [Url(ErrorMessage = "Picture link must be a valid URL")]
    public string? PictureLink { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the category was created
    /// </summary>
    /// <value>
    /// The UTC date and time when the category was first created in the system.
    /// </value>
    /// <example>2024-01-15T10:30:00Z</example>
    [Required]
    public DateTimeOffset CreatedDateTime { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the category was last updated
    /// </summary>
    /// <value>
    /// The UTC date and time when the category was last modified, or null if never updated.
    /// </value>
    /// <example>2024-01-20T14:45:00Z</example>
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
