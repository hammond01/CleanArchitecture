using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Contracts;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(RegisterRequest request);
    Task<Result<string>> LoginAsync(LoginRequest request);
}
