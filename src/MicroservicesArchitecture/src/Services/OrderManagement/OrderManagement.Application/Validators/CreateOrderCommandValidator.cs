using FluentValidation;
using OrderManagement.Application.Commands;

namespace OrderManagement.Application.Validators;

/// <summary>
/// Validator for CreateOrderCommand
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.CustomerInfo)
            .NotNull()
            .WithMessage("Customer information is required")
            .SetValidator(new CustomerInfoDtoValidator());

        RuleFor(x => x.OrderItems)
            .NotEmpty()
            .WithMessage("Order must contain at least one item");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new CreateOrderItemDtoValidator());

        RuleFor(x => x.ShippingAddress)
            .SetValidator(new AddressDtoValidator()!)
            .When(x => x.ShippingAddress != null);

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters");
    }
}

/// <summary>
/// Validator for CustomerInfoDto
/// </summary>
public class CustomerInfoDtoValidator : AbstractValidator<OrderManagement.Application.DTOs.CustomerInfoDto>
{
    public CustomerInfoDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(100)
            .WithMessage("Customer name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Customer email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(200)
            .WithMessage("Email cannot exceed 200 characters");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}

/// <summary>
/// Validator for CreateOrderItemDto
/// </summary>
public class CreateOrderItemDtoValidator : AbstractValidator<OrderManagement.Application.DTOs.CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero")
            .LessThanOrEqualTo(1000)
            .WithMessage("Quantity cannot exceed 1000");

        RuleFor(x => x.UnitPrice)
            .NotNull()
            .WithMessage("Unit price is required")
            .SetValidator(new MoneyDtoValidator());
    }
}

/// <summary>
/// Validator for AddressDto
/// </summary>
public class AddressDtoValidator : AbstractValidator<OrderManagement.Application.DTOs.AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street address is required")
            .MaximumLength(200)
            .WithMessage("Street address cannot exceed 200 characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required")
            .MaximumLength(100)
            .WithMessage("State cannot exceed 100 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("Zip code is required")
            .MaximumLength(20)
            .WithMessage("Zip code cannot exceed 20 characters");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .MaximumLength(100)
            .WithMessage("Country cannot exceed 100 characters");
    }
}

/// <summary>
/// Validator for MoneyDto
/// </summary>
public class MoneyDtoValidator : AbstractValidator<OrderManagement.Application.DTOs.MoneyDto>
{
    public MoneyDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Amount cannot exceed 1,000,000");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be 3 characters (ISO 4217 format)")
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be in uppercase ISO 4217 format (e.g., USD, EUR)");
    }
}
