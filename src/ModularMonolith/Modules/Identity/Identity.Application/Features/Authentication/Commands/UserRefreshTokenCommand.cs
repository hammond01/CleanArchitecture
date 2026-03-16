using BuildingBlocks.Application.CQRS;
using Identity.Domain.DTOs;

namespace Identity.Application.Features.Authentication.Commands;

/// <summary>
/// Command to refresh JWT token
/// </summary>
public record UserRefreshTokenCommand : ICommand<LoginResponseDto>
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
