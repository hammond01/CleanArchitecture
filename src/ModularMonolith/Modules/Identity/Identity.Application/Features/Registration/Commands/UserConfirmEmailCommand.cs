using BuildingBlocks.Application.CQRS;

namespace Identity.Application.Features.Registration.Commands;

/// <summary>
/// Command to confirm user email
/// </summary>
public record UserConfirmEmailCommand : ICommand<bool>
{
    public string UserId { get; init; } = null!;
    public string Token { get; init; } = null!;
}
