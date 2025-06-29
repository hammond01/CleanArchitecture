using ProductManager.Application.Common.Commands;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;
using ProductManager.Shared.DTOs.UserDto;
namespace ProductManager.Application.Feature.Identity.Commands;

public class UserLoginCommand : ICommand<ApiResponse>
{
    public UserLoginCommand(LoginRequest request)
    {
        Request = request;
    }
    public LoginRequest Request { get; set; }
}
internal class UserLoginCommandHandler : ICommandHandler<UserLoginCommand, ApiResponse>
{
    private readonly IIdentityRepository _identityRepository;
    public UserLoginCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }
    public async Task<ApiResponse> HandleAsync(UserLoginCommand command, CancellationToken cancellationToken = default)
        => await _identityRepository.Login(command.Request);
}
