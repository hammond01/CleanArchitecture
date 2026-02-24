using System.Security.Claims;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

/// <summary>
/// Repository implementation for identity operations
/// </summary>
public class IdentityRepository : IIdentityRepository
{
    private readonly IdentityDbContext _context;

    public IdentityRepository(IdentityDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Login user with username and password
    /// </summary>
    public async Task LoginAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User is not active");

        if (!user.IsEmailConfirmed)
            throw new UnauthorizedAccessException("User email is not confirmed");

        // TODO: Verify password hash using a password hasher service
        // For now, this is a placeholder
        if (user.PasswordHash != password) // This should use proper password verification
            throw new UnauthorizedAccessException("Invalid credentials");

        // Update last login
        user.LastLoginDateTime = DateTimeOffset.UtcNow;
        user.LoginAttempts = 0;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    public async Task RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

        if (token == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (!token.IsActive)
            throw new UnauthorizedAccessException("Refresh token is not active");

        // TODO: Validate access token and issue new tokens
        // This is a placeholder - real implementation would:
        // 1. Extract user claims from access token
        // 2. Generate new access token
        // 3. Optionally rotate refresh token
    }

    /// <summary>
    /// Logout authenticated user
    /// </summary>
    public async Task LogoutAsync(ClaimsPrincipal authenticatedUser, CancellationToken cancellationToken = default)
    {
        if (authenticatedUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value is not { } userIdString)
            throw new InvalidOperationException("User ID not found in claims");

        if (!Guid.TryParse(userIdString, out var userId))
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == userIdString, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"User '{userIdString}' not found");

            userId = user.Id;
        }

        // Revoke all active refresh tokens for the user
        var refreshTokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.Revoked == null)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.Revoked = DateTimeOffset.UtcNow;
        }

        _context.RefreshTokens.UpdateRange(refreshTokens);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Register new user with email and password
    /// </summary>
    public async Task RegisterAsync(string userName, string email, string password, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.UserName == userName || x.Email == email, cancellationToken);

        if (existingUser != null)
            throw new InvalidOperationException($"User with username '{userName}' or email '{email}' already exists");

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            // TODO: Hash password using a password hasher service
            PasswordHash = password, // This should be hashed!
            IsActive = true,
            IsEmailConfirmed = true,
            EmailConfirmedDateTime = DateTimeOffset.UtcNow,
            CreatedDateTime = DateTimeOffset.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Confirm user email with token
    /// </summary>
    public async Task ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            throw new InvalidOperationException("Invalid user ID format");

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userGuid, cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User with ID '{userId}' not found");

        if (user.EmailConfirmationToken != token)
            throw new InvalidOperationException("Invalid email confirmation token");

        user.IsEmailConfirmed = true;
        user.EmailConfirmationToken = null;
        user.EmailConfirmedDateTime = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Request password reset for user
    /// </summary>
    public async Task RequestPasswordResetAsync(string userName, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User '{userName}' not found");

        // TODO: Generate password reset token and send via email
        // This is a placeholder
        user.PasswordResetToken = Guid.NewGuid().ToString();
        user.PasswordResetTokenExpires = DateTimeOffset.UtcNow.AddHours(24);

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Reset password with reset token
    /// </summary>
    public async Task ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            throw new InvalidOperationException("Invalid user ID format");

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userGuid, cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User with ID '{userId}' not found");

        if (user.PasswordResetToken != token)
            throw new InvalidOperationException("Invalid password reset token");

        if (user.PasswordResetTokenExpires < DateTimeOffset.UtcNow)
            throw new InvalidOperationException("Password reset token has expired");

        // TODO: Hash new password using a password hasher service
        user.PasswordHash = newPassword; // This should be hashed!
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
