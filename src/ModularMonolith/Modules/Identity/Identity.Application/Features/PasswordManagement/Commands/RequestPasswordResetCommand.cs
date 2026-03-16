using BuildingBlocks.Application.CQRS;

namespace Identity.Application.Features.PasswordManagement.Commands;

/// <summary>
/// Command to request a password reset
/// </summary>
public record RequestPasswordResetCommand : ICommand<bool>
{
    public string UserName { get; init; } = null!;
}
