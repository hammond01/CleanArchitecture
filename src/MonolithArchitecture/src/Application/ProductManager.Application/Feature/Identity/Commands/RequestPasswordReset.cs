using ProductManager.Application.Common.Commands;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Feature.Identity.Commands;

public class RequestPasswordResetCommand : ICommand<ApiResponse>
{
    public RequestPasswordResetCommand(string userName)
    {
        UserName = userName;
    }

    public string UserName { get; set; }
}

internal class RequestPasswordResetCommandHandler : ICommandHandler<RequestPasswordResetCommand, ApiResponse>
{
    private readonly IIdentityRepository _identityRepository;
    public RequestPasswordResetCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }
    public async Task<ApiResponse> HandleAsync(RequestPasswordResetCommand command, CancellationToken cancellationToken = default)
        => await _identityRepository.RequestPasswordReset(command.UserName);
}
