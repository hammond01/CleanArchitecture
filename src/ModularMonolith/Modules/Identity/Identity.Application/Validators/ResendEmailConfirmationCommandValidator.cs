using FluentValidation;
using Identity.Application.Features.Registration.Commands;

namespace Identity.Application.Validators;

public class ResendEmailConfirmationCommandValidator : AbstractValidator<ResendEmailConfirmationCommand>
{
    public ResendEmailConfirmationCommandValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty();
    }
}
