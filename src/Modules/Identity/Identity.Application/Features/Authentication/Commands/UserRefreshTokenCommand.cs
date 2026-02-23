using BuildingBlocks.Application.CQRS;

namespace Identity.Application.Features.Authentication.Commands;

/// <summary>
/// Command to refresh JWT token
/// </summary>
public record UserRefreshTokenCommand : ICommand<string>
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
