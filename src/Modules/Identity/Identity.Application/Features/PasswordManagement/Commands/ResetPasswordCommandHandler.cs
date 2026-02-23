using BuildingBlocks.Application.CQRS;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.PasswordManagement.Commands;

/// <summary>
/// Handler for ResetPasswordCommand
/// </summary>
public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, bool>
{
    private readonly IIdentityRepository _identityRepository;

    public ResetPasswordCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<bool> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        await _identityRepository.ResetPasswordAsync(
            command.UserId,
            command.Token,
            command.Password,
            cancellationToken);

        return true;
    }
}
