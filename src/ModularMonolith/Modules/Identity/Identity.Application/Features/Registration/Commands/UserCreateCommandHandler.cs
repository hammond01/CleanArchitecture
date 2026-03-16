using BuildingBlocks.Application.CQRS;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Registration.Commands;

/// <summary>
/// Handler for UserCreateCommand
/// </summary>
public class UserCreateCommandHandler : ICommandHandler<UserCreateCommand, string>
{
    private readonly IIdentityRepository _identityRepository;

    public UserCreateCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<string> HandleAsync(UserCreateCommand command, CancellationToken cancellationToken = default)
    {
        var userId = await _identityRepository.RegisterAsync(
            command.UserName,
            command.Email,
            command.Password,
            command.FirstName,
            command.LastName,
            cancellationToken);

        return userId.ToString();
    }
}
