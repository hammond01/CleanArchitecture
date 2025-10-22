using Mediator;
using IdentityServer.Domain.Contracts;
using IdentityServer.Domain.Common;

namespace IdentityServer.Application.Handlers;

public partial class RegisterUserHandler : IRequestHandler<Commands.RegisterUserCommand, Result<Guid>>
{
    private readonly IIdentityService _identityService;

    public RegisterUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<Result<Guid>> Handle(Commands.RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RegisterAsync(request.Request);
    }
}
