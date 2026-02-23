using BuildingBlocks.Application.CQRS;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Authentication.Commands;

/// <summary>
/// Handler for UserRefreshTokenCommand
/// </summary>
public class UserRefreshTokenCommandHandler : ICommandHandler<UserRefreshTokenCommand, string>
{
    private readonly IIdentityRepository _identityRepository;

    public UserRefreshTokenCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<string> HandleAsync(UserRefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        await _identityRepository.RefreshTokenAsync(command.AccessToken, command.RefreshToken, cancellationToken);
        return command.AccessToken; // TODO: Return actual new token from repo
    }
}
