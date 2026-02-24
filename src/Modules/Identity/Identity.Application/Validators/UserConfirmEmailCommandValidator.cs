using FluentValidation;
using Identity.Application.Features.Registration.Commands;

namespace Identity.Application.Validators;

public class UserConfirmEmailCommandValidator : AbstractValidator<UserConfirmEmailCommand>
{
    public UserConfirmEmailCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
    }
}
