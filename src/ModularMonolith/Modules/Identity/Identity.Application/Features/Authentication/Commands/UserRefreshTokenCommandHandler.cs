using BuildingBlocks.Application.CQRS;
using Identity.Domain.DTOs;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Authentication.Commands;

/// <summary>
/// Handler for UserRefreshTokenCommand
/// </summary>
public class UserRefreshTokenCommandHandler : ICommandHandler<UserRefreshTokenCommand, LoginResponseDto>
{
    private readonly IIdentityRepository _identityRepository;

    public UserRefreshTokenCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<LoginResponseDto> HandleAsync(UserRefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        return await _identityRepository.RefreshTokenAsync(
            command.AccessToken,
            command.RefreshToken,
            cancellationToken);
    }
}
