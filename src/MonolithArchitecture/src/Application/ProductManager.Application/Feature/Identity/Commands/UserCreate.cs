using ProductManager.Application.Common.Commands;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;
using ProductManager.Shared.DTOs.UserDto;
namespace ProductManager.Application.Feature.Identity.Commands;

public class UserCreateCommand : ICommand<ApiResponse>
{
    public UserCreateCommand(RegisterRequest request)
    {
        Request = request;
    }
    public RegisterRequest Request { get; set; }
}
internal class UserCreateCommandHandler : ICommandHandler<UserCreateCommand, ApiResponse>
{
    private readonly IIdentityRepository _identityRepository;
    public UserCreateCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }
    public async Task<ApiResponse> HandleAsync(UserCreateCommand command, CancellationToken cancellationToken = default)
        => await _identityRepository.Register(command.Request);
}
