using FluentValidation;
using Identity.Application.Features.Authentication.Commands;

namespace Identity.Application.Validators;

public class UserLogoutCommandValidator : AbstractValidator<UserLogoutCommand>
{
    public UserLogoutCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
