using FluentValidation;
using ProductCatalog.Application.Features.Products.Commands;

namespace ProductCatalog.Application.Validators.Products;

/// <summary>
/// Validator for CreateProductCommand
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required");

        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("Supplier ID is required");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0")
            .LessThan(1000000).WithMessage("Unit price must be less than 1,000,000");

        RuleFor(x => x.UnitsInStock)
            .GreaterThanOrEqualTo(0).WithMessage("Units in stock cannot be negative");

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder level cannot be negative");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).WithMessage("Weight cannot be negative");

        RuleFor(x => x.ImageUrl)
            .Must(BeValidUrl).WithMessage("Image URL must be a valid URL")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}

/// <summary>
/// Validator for UpdateProductCommand
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0")
            .LessThan(1000000).WithMessage("Unit price must be less than 1,000,000");

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder level cannot be negative");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).WithMessage("Weight cannot be negative");

        RuleFor(x => x.ImageUrl)
            .Must(BeValidUrl).WithMessage("Image URL must be a valid URL")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}

/// <summary>
/// Validator for UpdateProductStockCommand
/// </summary>
public class UpdateProductStockCommandValidator : AbstractValidator<UpdateProductStockCommand>
{
    public UpdateProductStockCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.NewStock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");
    }
}

/// <summary>
/// Validator for ReduceProductStockCommand
/// </summary>
public class ReduceProductStockCommandValidator : AbstractValidator<ReduceProductStockCommand>
{
    public ReduceProductStockCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}

/// <summary>
/// Validator for IncreaseProductStockCommand
/// </summary>
public class IncreaseProductStockCommandValidator : AbstractValidator<IncreaseProductStockCommand>
{
    public IncreaseProductStockCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}

/// <summary>
/// Validator for commands that only need ID
/// </summary>
public class ProductIdCommandValidator<T> : AbstractValidator<T> where T : class
{
    public ProductIdCommandValidator()
    {
        RuleFor(x => GetId(x))
            .NotEmpty().WithMessage("Product ID is required");
    }

    private static string GetId(T command)
    {
        var property = typeof(T).GetProperty("Id");
        return property?.GetValue(command)?.ToString() ?? string.Empty;
    }
}
