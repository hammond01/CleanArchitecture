using ProductManager.Application.Common.Commands;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;
using ProductManager.Shared.DTOs.UserDto;
namespace ProductManager.Application.Feature.Identity.Commands;

public class UserConfirmEmailCommand : ICommand<ApiResponse>
{
    public UserConfirmEmailCommand(ConfirmEmailDto request)
    {
        Request = request;
    }
    public ConfirmEmailDto Request { get; set; }
}
internal class UserConfirmEmailCommandHandler : ICommandHandler<UserConfirmEmailCommand, ApiResponse>
{
    private readonly IIdentityRepository _identityRepository;
    public UserConfirmEmailCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }
    public async Task<ApiResponse> HandleAsync(UserConfirmEmailCommand command, CancellationToken cancellationToken = default)
        => await _identityRepository.ConfirmEmail(command.Request);
}
