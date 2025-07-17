using FluentValidation;
using ProductCatalog.Application.Products.Commands;
using ProductCatalog.Application.Products.Queries;

namespace ProductCatalog.Application.Products.Validators;

/// <summary>
/// Create product command validator
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(40).WithMessage("Product name cannot exceed 40 characters");

        RuleFor(x => x.CategoryId)
            .MaximumLength(50).WithMessage("Category ID cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.CategoryId));

        RuleFor(x => x.SupplierId)
            .MaximumLength(50).WithMessage("Supplier ID cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.SupplierId));

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative")
            .When(x => x.UnitPrice.HasValue);

        RuleFor(x => x.UnitsInStock)
            .GreaterThanOrEqualTo((short)0).WithMessage("Units in stock cannot be negative")
            .When(x => x.UnitsInStock.HasValue);

        RuleFor(x => x.UnitsOnOrder)
            .GreaterThanOrEqualTo((short)0).WithMessage("Units on order cannot be negative")
            .When(x => x.UnitsOnOrder.HasValue);

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo((short)0).WithMessage("Reorder level cannot be negative")
            .When(x => x.ReorderLevel.HasValue);
    }
}

/// <summary>
/// Update product command validator
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required")
            .MaximumLength(50).WithMessage("Product ID cannot exceed 50 characters");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(40).WithMessage("Product name cannot exceed 40 characters");

        RuleFor(x => x.CategoryId)
            .MaximumLength(50).WithMessage("Category ID cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.CategoryId));

        RuleFor(x => x.SupplierId)
            .MaximumLength(50).WithMessage("Supplier ID cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.SupplierId));

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative")
            .When(x => x.UnitPrice.HasValue);

        RuleFor(x => x.UnitsInStock)
            .GreaterThanOrEqualTo((short)0).WithMessage("Units in stock cannot be negative")
            .When(x => x.UnitsInStock.HasValue);

        RuleFor(x => x.UnitsOnOrder)
            .GreaterThanOrEqualTo((short)0).WithMessage("Units on order cannot be negative")
            .When(x => x.UnitsOnOrder.HasValue);

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo((short)0).WithMessage("Reorder level cannot be negative")
            .When(x => x.ReorderLevel.HasValue);
    }
}

/// <summary>
/// Delete product command validator
/// </summary>
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required")
            .MaximumLength(50).WithMessage("Product ID cannot exceed 50 characters");
    }
}

/// <summary>
/// Get product by ID query validator
/// </summary>
public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required")
            .MaximumLength(50).WithMessage("Product ID cannot exceed 50 characters");
    }
}

/// <summary>
/// Get products by category query validator
/// </summary>
public class GetProductsByCategoryQueryValidator : AbstractValidator<GetProductsByCategoryQuery>
{
    public GetProductsByCategoryQueryValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required")
            .MaximumLength(50).WithMessage("Category ID cannot exceed 50 characters");
    }
}

/// <summary>
/// Get products by supplier query validator
/// </summary>
public class GetProductsBySupplierQueryValidator : AbstractValidator<GetProductsBySupplierQuery>
{
    public GetProductsBySupplierQueryValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("Supplier ID is required")
            .MaximumLength(50).WithMessage("Supplier ID cannot exceed 50 characters");
    }
}
