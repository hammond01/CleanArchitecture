using BuildingBlocks.Application.CQRS;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Authentication.Commands;

/// <summary>
/// Handler for UserLogoutCommand
/// </summary>
public class UserLogoutCommandHandler : ICommandHandler<UserLogoutCommand, bool>
{
    private readonly IIdentityRepository _identityRepository;

    public UserLogoutCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<bool> HandleAsync(UserLogoutCommand command, CancellationToken cancellationToken = default)
    {
        await _identityRepository.LogoutAsync(command.UserId, cancellationToken);
        return true;
    }
}
