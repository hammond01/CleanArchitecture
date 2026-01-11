using Catalog.Application.Features.Categories.Commands;
using FluentValidation;

namespace Catalog.Application.Validators;

/// <summary>
/// Validator for CreateOrUpdateCategoryCommand
/// </summary>
public class CreateOrUpdateCategoryCommandValidator : AbstractValidator<CreateOrUpdateCategoryCommand>
{
    public CreateOrUpdateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(15).WithMessage("Category name must not exceed 15 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.PictureLink)
            .MaximumLength(500).WithMessage("Picture link must not exceed 500 characters")
            .Must(BeAValidUrl).WithMessage("Picture link must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.PictureLink));
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
