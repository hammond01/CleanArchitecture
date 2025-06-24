// FluentValidation setup for advanced validation
using FluentValidation;
using ProductManager.Shared.DTOs.CustomerDto;

namespace ProductManager.Application.Validators;

/// <summary>
/// Advanced validation rules for Customer DTOs
/// </summary>
public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Customer ID is required")
            .Length(5).WithMessage("Customer ID must be exactly 5 characters")
            .Matches("^[A-Z]{5}$").WithMessage("Customer ID must be 5 uppercase letters");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(40).WithMessage("Company name cannot exceed 40 characters")
            .Must(BeUniqueCompanyName).WithMessage("Company name already exists");

        RuleFor(x => x.ContactName)
            .MaximumLength(30).WithMessage("Contact name cannot exceed 30 characters")
            .Must(BeValidName).WithMessage("Contact name contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.ContactName));

        RuleFor(x => x.Phone)
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Phone number format is invalid")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.PostalCode)
            .Must(BeValidPostalCode).WithMessage("Invalid postal code format")
            .When(x => !string.IsNullOrEmpty(x.PostalCode));
    }

    private bool BeUniqueCompanyName(string companyName)
    {
        // Database check for unique company name
        // This would be injected service in real implementation
        return true; // Placeholder
    }

    private bool BeValidName(string name)
    {
        return !string.IsNullOrEmpty(name) && name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '.' || c == '-');
    }    private bool BeValidPostalCode(string? postalCode)
    {
        if (string.IsNullOrEmpty(postalCode))
            return true; // Let the NotEmpty rule handle null/empty validation

        // Different validation patterns for different countries
        var patterns = new[]
        {
            @"^\d{5}$", // US
            @"^[A-Z]\d[A-Z] \d[A-Z]\d$", // Canada
            @"^\d{5}-\d{4}$" // US extended
        };

        return patterns.Any(pattern => System.Text.RegularExpressions.Regex.IsMatch(postalCode, pattern));
    }
}

public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(40).WithMessage("Company name cannot exceed 40 characters");

        RuleFor(x => x.ContactName)
            .MaximumLength(30).WithMessage("Contact name cannot exceed 30 characters")
            .When(x => !string.IsNullOrEmpty(x.ContactName));

        RuleFor(x => x.Phone)
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Phone number format is invalid")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}
