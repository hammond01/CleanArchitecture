using System.Security.Claims;
using ProductManager.Application.Common.Commands;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;
namespace ProductManager.Application.Feature.Identity.Commands;

public class UserLogOutCommand : ICommand<ApiResponse>
{
    public UserLogOutCommand(ClaimsPrincipal authenticatedUser)
    {
        AuthenticatedUser = authenticatedUser;
    }
    public ClaimsPrincipal AuthenticatedUser { get; set; }
}
internal class UserLogOutHandler : ICommandHandler<UserLogOutCommand, ApiResponse>
{
    private readonly IIdentityRepository _identityRepository;
    public UserLogOutHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }
    public async Task<ApiResponse> HandleAsync(UserLogOutCommand command, CancellationToken cancellationToken = default)
        => await _identityRepository.Logout(command.AuthenticatedUser);
}
