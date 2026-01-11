using Catalog.Application.Features.Categories.Commands;
using FluentValidation;

namespace Catalog.Application.Validators;

/// <summary>
/// Validator for DeleteCategoryCommand
/// </summary>
public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required");
    }
}
