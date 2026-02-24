using FluentValidation;
using Identity.Application.Features.Authentication.Commands;

namespace Identity.Application.Validators;

public class UserRefreshTokenCommandValidator : AbstractValidator<UserRefreshTokenCommand>
{
    public UserRefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
