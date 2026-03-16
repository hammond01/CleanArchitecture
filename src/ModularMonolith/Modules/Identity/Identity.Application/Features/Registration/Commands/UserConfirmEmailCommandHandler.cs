using BuildingBlocks.Application.CQRS;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Registration.Commands;

/// <summary>
/// Handler for UserConfirmEmailCommand
/// </summary>
public class UserConfirmEmailCommandHandler : ICommandHandler<UserConfirmEmailCommand, bool>
{
    private readonly IIdentityRepository _identityRepository;

    public UserConfirmEmailCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<bool> HandleAsync(UserConfirmEmailCommand command, CancellationToken cancellationToken = default)
    {
        await _identityRepository.ConfirmEmailAsync(command.UserId, command.Token, cancellationToken);
        return true;
    }
}
