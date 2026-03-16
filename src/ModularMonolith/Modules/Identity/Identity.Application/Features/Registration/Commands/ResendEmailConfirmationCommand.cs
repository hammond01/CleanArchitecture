using BuildingBlocks.Application.CQRS;

namespace Identity.Application.Features.Registration.Commands;

/// <summary>
/// Command to resend confirmation email
/// </summary>
public record ResendEmailConfirmationCommand : ICommand<bool>
{
    public string UserNameOrEmail { get; init; } = null!;
}
