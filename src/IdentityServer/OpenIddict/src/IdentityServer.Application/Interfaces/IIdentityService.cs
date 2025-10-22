using IdentityServer.Application.DTOs;
using IdentityServer.Domain.Common;

namespace IdentityServer.Application.Interfaces;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(RegisterRequest request);
    Task<Result<string>> LoginAsync(LoginRequest request);
}
