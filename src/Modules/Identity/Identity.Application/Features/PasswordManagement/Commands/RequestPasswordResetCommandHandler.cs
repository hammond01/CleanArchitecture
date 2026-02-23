using BuildingBlocks.Application.CQRS;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.PasswordManagement.Commands;

/// <summary>
/// Handler for RequestPasswordResetCommand
/// </summary>
public class RequestPasswordResetCommandHandler : ICommandHandler<RequestPasswordResetCommand, bool>
{
    private readonly IIdentityRepository _identityRepository;

    public RequestPasswordResetCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<bool> HandleAsync(RequestPasswordResetCommand command, CancellationToken cancellationToken = default)
    {
        await _identityRepository.RequestPasswordResetAsync(command.UserName, cancellationToken);
        return true;
    }
}
