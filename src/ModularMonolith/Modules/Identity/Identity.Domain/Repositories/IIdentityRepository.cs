using Identity.Domain.DTOs;

namespace Identity.Domain.Repositories;

/// <summary>
/// Repository interface for identity operations
/// </summary>
public interface IIdentityRepository
{
    /// <summary>
    /// Login user with username and password
    /// </summary>
    Task<LoginResponseDto> LoginAsync(
        string userName,
        string password,
        bool rememberMe,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    Task<LoginResponseDto> RefreshTokenAsync(
        string accessToken,
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout authenticated user
    /// </summary>
    Task LogoutAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Register new user with email and password
    /// </summary>
    Task<Guid> RegisterAsync(
        string userName,
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirm user email with token
    /// </summary>
    Task ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Request password reset for user
    /// </summary>
    Task RequestPasswordResetAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset password with reset token
    /// </summary>
    Task ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resend confirmation email
    /// </summary>
    Task ResendEmailConfirmationAsync(string userName, CancellationToken cancellationToken = default);
}
