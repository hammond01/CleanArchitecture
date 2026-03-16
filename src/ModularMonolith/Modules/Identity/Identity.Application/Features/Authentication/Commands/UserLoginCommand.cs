using BuildingBlocks.Application.CQRS;
using Identity.Domain.DTOs;

namespace Identity.Application.Features.Authentication.Commands;

/// <summary>
/// Command to login a user
/// </summary>
public record UserLoginCommand : ICommand<LoginResponseDto>
{
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public bool RememberMe { get; init; }
}
