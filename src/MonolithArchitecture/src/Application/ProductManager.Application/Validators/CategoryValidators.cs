using FluentValidation;
using ProductManager.Shared.DTOs.CategoryDto;

namespace ProductManager.Application.Validators;

/// <summary>
/// Advanced validation rules for Category DTOs with business logic validation
/// </summary>
public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category name is required")
            .Length(1, 150).WithMessage("Category name must be between 1 and 150 characters")
            .Matches(@"^[a-zA-Z0-9\s\-&().,]+$").WithMessage("Category name contains invalid characters")
            .Must(BeUniqueWithinBoundaries).WithMessage("Category name should be unique within reasonable boundaries")
            .Must(NotContainReservedWords).WithMessage("Category name cannot contain reserved words")
            .Must(BeValidCategoryName).WithMessage("Category name must be appropriate for business use");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Description cannot exceed 250 characters")
            .Must(BeValidDescription).WithMessage("Description contains inappropriate content")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    /// <summary>
    /// Validates that category name is unique within reasonable boundaries
    /// </summary>
    private bool BeUniqueWithinBoundaries(string categoryName)
    {
        // In a real implementation, this would check against database
        // For now, we'll implement basic validation
        return !string.IsNullOrEmpty(categoryName);
    }

    /// <summary>
    /// Validates that category name doesn't contain reserved words
    /// </summary>
    private bool NotContainReservedWords(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
            return true;

        var reservedWords = new[] { "admin", "system", "root", "null", "undefined", "test", "temp", "temporary" };
        return !reservedWords.Any(word => categoryName.ToLower().Contains(word.ToLower()));
    }

    /// <summary>
    /// Validates that category name is appropriate for business use
    /// </summary>
    private bool BeValidCategoryName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
            return false;

        // Check for inappropriate patterns
        var inappropriatePatterns = new[] { @"^\d+$", @"^[^a-zA-Z]*$", @"^[\s]*$" };
        return !inappropriatePatterns.Any(pattern => System.Text.RegularExpressions.Regex.IsMatch(categoryName, pattern));
    }

    /// <summary>
    /// Validates that description is appropriate for business use
    /// </summary>
    private bool BeValidDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
            return true;

        // Check for inappropriate content (basic implementation)
        var inappropriateWords = new[] { "spam", "fake", "test123", "lorem ipsum" };
        return !inappropriateWords.Any(word => description.ToLower().Contains(word.ToLower()));
    }
}

/// <summary>
/// Advanced validation rules for updating Category DTOs
/// </summary>
public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category name is required")
            .Length(1, 150).WithMessage("Category name must be between 1 and 150 characters")
            .Matches(@"^[a-zA-Z0-9\s\-&().,]+$").WithMessage("Category name contains invalid characters")
            .Must(NotContainReservedWords).WithMessage("Category name cannot contain reserved words")
            .Must(BeValidCategoryName).WithMessage("Category name must be appropriate for business use");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Description cannot exceed 250 characters")
            .Must(BeValidDescription).WithMessage("Description contains inappropriate content")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    /// <summary>
    /// Validates that category name doesn't contain reserved words
    /// </summary>
    private bool NotContainReservedWords(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
            return true;

        var reservedWords = new[] { "admin", "system", "root", "null", "undefined", "test", "temp", "temporary" };
        return !reservedWords.Any(word => categoryName.ToLower().Contains(word.ToLower()));
    }

    /// <summary>
    /// Validates that category name is appropriate for business use
    /// </summary>
    private bool BeValidCategoryName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
            return false;

        // Check for inappropriate patterns
        var inappropriatePatterns = new[] { @"^\d+$", @"^[^a-zA-Z]*$", @"^[\s]*$" };
        return !inappropriatePatterns.Any(pattern => System.Text.RegularExpressions.Regex.IsMatch(categoryName, pattern));
    }

    /// <summary>
    /// Validates that description is appropriate for business use
    /// </summary>
    private bool BeValidDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
            return true;

        // Check for inappropriate content (basic implementation)
        var inappropriateWords = new[] { "spam", "fake", "test123", "lorem ipsum" };
        return !inappropriateWords.Any(word => description.ToLower().Contains(word.ToLower()));
    }
}

/// <summary>
/// Advanced validation rules for Category ID parameters
/// </summary>
public class CategoryIdValidator : AbstractValidator<string>
{
    public CategoryIdValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Category ID is required")
            .Length(1, 50).WithMessage("Category ID must be between 1 and 50 characters")
            .Must(BeValidIdFormat).WithMessage("Category ID format is invalid")
            .Must(NotBeTestId).WithMessage("Test IDs are not allowed in production");
    }

    /// <summary>
    /// Validates that the ID format is valid (ULID, GUID, or custom format)
    /// </summary>
    private bool BeValidIdFormat(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        // Check for ULID format (26 characters, base32)
        if (id.Length == 26 && id.All(c => "0123456789ABCDEFGHJKMNPQRSTVWXYZ".Contains(c)))
            return true;

        // Check for GUID format
        if (Guid.TryParse(id, out _))
            return true;

        // Check for custom alphanumeric format
        if (id.All(c => char.IsLetterOrDigit(c) || c == '-') && id.Length <= 50)
            return true;

        return false;
    }

    /// <summary>
    /// Validates that the ID is not a test ID
    /// </summary>
    private bool NotBeTestId(string id)
    {
        if (string.IsNullOrEmpty(id))
            return true;

        var testPatterns = new[] { "test", "temp", "sample", "demo", "example" };
        return !testPatterns.Any(pattern => id.ToLower().Contains(pattern));
    }
}
