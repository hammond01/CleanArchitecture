using BuildingBlocks.Application.CQRS;

namespace Identity.Application.Features.Authentication.Commands;

/// <summary>
/// Command to logout a user
/// </summary>
public record UserLogoutCommand : ICommand<bool>
{
    public string UserId { get; init; } = null!;
}
