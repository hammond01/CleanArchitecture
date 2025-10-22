using IdentityServer.Application.DTOs;
using IdentityServer.Domain.Common;
using Mediator;

namespace IdentityServer.Application.Commands;

public partial record LoginUserCommand(LoginRequest Request) : IRequest<Result<string>>;
