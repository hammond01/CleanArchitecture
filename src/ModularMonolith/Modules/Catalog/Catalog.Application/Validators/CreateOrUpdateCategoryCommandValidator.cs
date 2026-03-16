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
            .MaximumLength(150).WithMessage("Category name must not exceed 150 characters");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Description must not exceed 250 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.PictureLink)
            .MaximumLength(100).WithMessage("Picture link must not exceed 100 characters")
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
