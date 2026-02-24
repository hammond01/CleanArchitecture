using FluentValidation;
using Identity.Application.Features.PasswordManagement.Commands;

namespace Identity.Application.Validators;

public class RequestPasswordResetCommandValidator : AbstractValidator<RequestPasswordResetCommand>
{
    public RequestPasswordResetCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
    }
}
