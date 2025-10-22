using IdentityServer.Application.DTOs;
using IdentityServer.Domain.Common;
using Mediator;

namespace IdentityServer.Application.Commands;

public partial record RegisterUserCommand(RegisterRequest Request) : IRequest<Result<Guid>>;
