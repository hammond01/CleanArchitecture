using BuildingBlocks.Application.CQRS;
using Identity.Domain.DTOs;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Authentication.Commands;

/// <summary>
/// Handler for UserLoginCommand
/// </summary>
public class UserLoginCommandHandler : ICommandHandler<UserLoginCommand, LoginResponseDto>
{
    private readonly IIdentityRepository _identityRepository;

    public UserLoginCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<LoginResponseDto> HandleAsync(UserLoginCommand command, CancellationToken cancellationToken = default)
    {
        return await _identityRepository.LoginAsync(
            command.UserName,
            command.Password,
            command.RememberMe,
            cancellationToken);
    }
}
