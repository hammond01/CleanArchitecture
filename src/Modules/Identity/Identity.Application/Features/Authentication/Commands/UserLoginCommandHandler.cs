using BuildingBlocks.Application.CQRS;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Authentication.Commands;

/// <summary>
/// Handler for UserLoginCommand
/// </summary>
public class UserLoginCommandHandler : ICommandHandler<UserLoginCommand, string>
{
    private readonly IIdentityRepository _identityRepository;

    public UserLoginCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<string> HandleAsync(UserLoginCommand command, CancellationToken cancellationToken = default)
    {
        await _identityRepository.LoginAsync(command.UserName, command.Password, cancellationToken);
        return command.UserName; // TODO: Return actual JWT token from repo
    }
}
