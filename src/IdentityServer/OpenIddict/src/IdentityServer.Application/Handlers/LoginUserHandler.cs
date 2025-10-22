using Mediator;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.Common;

namespace IdentityServer.Application.Handlers;

public partial class LoginUserHandler : IRequestHandler<Commands.LoginUserCommand, Result<string>>
{
    private readonly IIdentityService _identityService;

    public LoginUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result<string>> Handle(Commands.LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(request.Request);
    }
}
