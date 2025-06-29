using System.Security.Claims;
using ProductManager.Domain.Common;
using ProductManager.Shared.DTOs.UserDto;

namespace ProductManager.Domain.Repositories;

public interface IIdentityRepository
{
    Task<ApiResponse> Login(LoginRequest parameters);
    Task<ApiResponse> RefreshToken(string accessToken, string refreshToken);
    Task<ApiResponse> Logout(ClaimsPrincipal authenticatedUser);
    Task<ApiResponse> Register(RegisterRequest parameters);
    Task<ApiResponse> ConfirmEmail(ConfirmEmailDto parameters);
}
