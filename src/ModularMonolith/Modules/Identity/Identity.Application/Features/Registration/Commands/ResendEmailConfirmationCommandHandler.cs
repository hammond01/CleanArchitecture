using BuildingBlocks.Application.CQRS;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Registration.Commands;

/// <summary>
/// Handler for ResendEmailConfirmationCommand
/// </summary>
public class ResendEmailConfirmationCommandHandler : ICommandHandler<ResendEmailConfirmationCommand, bool>
{
    private readonly IIdentityRepository _identityRepository;

    public ResendEmailConfirmationCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<bool> HandleAsync(ResendEmailConfirmationCommand command, CancellationToken cancellationToken = default)
    {
        await _identityRepository.ResendEmailConfirmationAsync(command.UserNameOrEmail, cancellationToken);
        return true;
    }
}
