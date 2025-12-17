using ProductManager.Application.Common.Commands;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;
using ProductManager.Shared.DTOs.UserDto;

namespace ProductManager.Application.Feature.Identity.Commands;

public class ResetPasswordCommand : ICommand<ApiResponse>
{
    public ResetPasswordCommand(ResetPasswordRequestDto request)
    {
        Request = request;
    }

    public ResetPasswordRequestDto Request { get; set; }
}

internal class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, ApiResponse>
{
    private readonly IIdentityRepository _identityRepository;
    public ResetPasswordCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }
    public async Task<ApiResponse> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
        => await _identityRepository.ResetPassword(command.Request);
}
