using ProductManager.Shared.DTOs.UserDto;
namespace ProductManager.Blazor.Services.Impl;
public interface IIdentityServices
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
