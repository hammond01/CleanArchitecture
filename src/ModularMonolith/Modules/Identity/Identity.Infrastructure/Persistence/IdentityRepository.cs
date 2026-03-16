using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Identity.Application.Services;
using Identity.Domain.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Events;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity.Infrastructure.Persistence;

/// <summary>
/// Repository implementation for identity operations
/// </summary>
public class IdentityRepository : IIdentityRepository
{
    private readonly IdentityDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly EmailSettings _emailSettings;
    private readonly IdentitySecuritySettings _securitySettings;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<IdentityRepository> _logger;

    public IdentityRepository(
        IdentityDbContext context,
        IPasswordHasher<User> passwordHasher,
        IEmailService emailService,
        IEmailTemplateProvider emailTemplateProvider,
        IOptions<EmailSettings> emailSettings,
        IOptions<IdentitySecuritySettings> securitySettings,
        IOptions<JwtSettings> jwtSettings,
        ILogger<IdentityRepository> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _emailTemplateProvider = emailTemplateProvider;
        _emailSettings = emailSettings.Value;
        _securitySettings = securitySettings.Value;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Login user with username and password
    /// </summary>
    public async Task<LoginResponseDto> LoginAsync(
        string userName,
        string password,
        bool rememberMe,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User is not active");

        if (user.LockoutEndDateTime.HasValue && user.LockoutEndDateTime > DateTimeOffset.UtcNow)
            throw new UnauthorizedAccessException("User is locked out");

        if (!user.IsEmailConfirmed)
            throw new UnauthorizedAccessException("User email is not confirmed");

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification == PasswordVerificationResult.Failed)
        {
            user.LoginAttempts++;
            if (user.LoginAttempts >= _securitySettings.MaxLoginAttempts)
            {
                user.LockoutEndDateTime = DateTimeOffset.UtcNow.AddMinutes(_securitySettings.LockoutMinutes);
                _logger.LogWarning("User {UserName} locked out after {Attempts} attempts", user.UserName, user.LoginAttempts);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
        }

        // Update last login
        user.LastLoginDateTime = DateTimeOffset.UtcNow;
        user.LoginAttempts = 0;
        user.LockoutEndDateTime = null;

        RevokeActiveRefreshTokens(user);
        var refreshToken = CreateRefreshToken(user.Id, rememberMe);
        user.AddDomainEvent(new UserLoginEvent
        {
            UserId = user.Id,
            UserName = user.UserName,
            LoginDateTime = user.LastLoginDateTime ?? DateTimeOffset.UtcNow
        });
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserName} logged in", user.UserName);
        return BuildLoginResponse(user, refreshToken);
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    public async Task<LoginResponseDto> RefreshTokenAsync(
        string accessToken,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

        if (token == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (!token.IsActive)
            throw new UnauthorizedAccessException("Refresh token is not active");

        var user = token.User ?? throw new UnauthorizedAccessException("Refresh token user was not found");
        if (!user.IsActive || !user.IsEmailConfirmed)
            throw new UnauthorizedAccessException("User is not allowed to refresh tokens");

        var principal = GetPrincipalFromToken(accessToken);
        var accessTokenUserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.Equals(accessTokenUserId, user.Id.ToString(), StringComparison.Ordinal))
            throw new UnauthorizedAccessException("Access token does not match refresh token owner");

        token.Revoked = DateTimeOffset.UtcNow;
        token.UpdatedDateTime = DateTimeOffset.UtcNow;

        var rememberMe = token.Expires > token.CreatedDateTime.AddDays(_jwtSettings.RefreshTokenDays);
        var replacementToken = CreateRefreshToken(user.Id, rememberMe);
        await _context.RefreshTokens.AddAsync(replacementToken, cancellationToken);
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refreshed tokens for user {UserName}", user.UserName);
        return BuildLoginResponse(user, replacementToken);
    }

    /// <summary>
    /// Logout authenticated user
    /// </summary>
    public async Task LogoutAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
            throw new InvalidOperationException("Invalid user ID format");

        // Revoke all active refresh tokens for the user
        var refreshTokens = await _context.RefreshTokens
            .Where(x => x.UserId == parsedUserId && x.Revoked == null)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.Revoked = DateTimeOffset.UtcNow;
            token.UpdatedDateTime = DateTimeOffset.UtcNow;
        }

        _context.RefreshTokens.UpdateRange(refreshTokens);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} logged out", parsedUserId);
    }

    /// <summary>
    /// Register new user with email and password
    /// </summary>
    public async Task<Guid> RegisterAsync(
        string userName,
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
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
            PasswordHash = string.Empty,
            IsActive = true,
            IsEmailConfirmed = false,
            CreatedDateTime = DateTimeOffset.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        var confirmationToken = GenerateToken();
        user.EmailConfirmationToken = confirmationToken;
        user.AddDomainEvent(new UserRegisteredEvent
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            RegisteredDateTime = user.CreatedDateTime
        });

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserName} registered", user.UserName);

        var confirmationEmail = _emailTemplateProvider.BuildConfirmationEmail(user, confirmationToken, _emailSettings.BaseUrl);
        await _emailService.SendAsync(confirmationEmail, cancellationToken);
        return user.Id;
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

        if (IsConfirmationTokenExpired(token))
            throw new InvalidOperationException("Email confirmation token has expired");

        user.IsEmailConfirmed = true;
        user.EmailConfirmationToken = null;
        user.EmailConfirmedDateTime = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserName} confirmed email", user.UserName);

        var welcomeEmail = _emailTemplateProvider.BuildWelcomeEmail(user, _emailSettings.BaseUrl);
        await _emailService.SendAsync(welcomeEmail, cancellationToken);
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

        if (IsResetRequestThrottled(user))
            throw new InvalidOperationException("Password reset already requested. Please wait before requesting again");

        var resetToken = GenerateToken();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpires = DateTimeOffset.UtcNow.AddMinutes(_securitySettings.PasswordResetTokenMinutes);

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset requested for user {UserName}", user.UserName);

        var resetEmail = _emailTemplateProvider.BuildPasswordResetEmail(user, resetToken, _emailSettings.BaseUrl);
        await _emailService.SendAsync(resetEmail, cancellationToken);
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

        var reuseCheck = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, newPassword);
        if (reuseCheck != PasswordVerificationResult.Failed)
            throw new InvalidOperationException("New password must be different from current password");

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset completed for user {UserName}", user.UserName);

        var changedEmail = _emailTemplateProvider.BuildPasswordChangedEmail(user, _emailSettings.BaseUrl);
        await _emailService.SendAsync(changedEmail, cancellationToken);
    }

    /// <summary>
    /// Resend confirmation email for a user
    /// </summary>
    public async Task ResendEmailConfirmationAsync(string userName, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.UserName == userName || x.Email == userName, cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User '{userName}' not found");

        if (user.IsEmailConfirmed)
            throw new InvalidOperationException("Email is already confirmed");

        var confirmationToken = GenerateToken();
        user.EmailConfirmationToken = confirmationToken;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Resent confirmation email for user {UserName}", user.UserName);

        var confirmationEmail = _emailTemplateProvider.BuildConfirmationEmail(user, confirmationToken, _emailSettings.BaseUrl);
        await _emailService.SendAsync(confirmationEmail, cancellationToken);
    }

    private static string GenerateToken()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        var randomPart = Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

        return $"{timestamp}_{randomPart}";
    }

    private (string Token, DateTimeOffset ExpiresAtUtc) CreateAccessToken(User user)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName)
        };

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return (token, expiresAt);
    }

    private RefreshToken CreateRefreshToken(Guid userId, bool rememberMe)
    {
        var lifetimeDays = rememberMe
            ? _jwtSettings.RememberMeRefreshTokenDays
            : _jwtSettings.RefreshTokenDays;

        return new RefreshToken
        {
            Id = Guid.NewGuid().ToString(),
            Token = GenerateToken(),
            UserId = userId,
            Expires = DateTimeOffset.UtcNow.AddDays(lifetimeDays),
            CreatedDateTime = DateTimeOffset.UtcNow
        };
    }

    private LoginResponseDto BuildLoginResponse(User user, RefreshToken refreshToken)
    {
        var (accessToken, expiresAtUtc) = CreateAccessToken(user);
        return new LoginResponseDto
        {
            UserId = user.Id.ToString(),
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            RequiresTwoFactor = false,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    private ClaimsPrincipal GetPrincipalFromToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };

        return tokenHandler.ValidateToken(accessToken, validationParameters, out _);
    }

    private static void RevokeActiveRefreshTokens(User user)
    {
        foreach (var refreshToken in user.RefreshTokens.Where(x => x.IsActive))
        {
            refreshToken.Revoked = DateTimeOffset.UtcNow;
            refreshToken.UpdatedDateTime = DateTimeOffset.UtcNow;
        }
    }

    private bool IsConfirmationTokenExpired(string token)
    {
        var parts = token.Split('_', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return false;

        if (!long.TryParse(parts[0], out var unixSeconds))
            return false;

        var issuedAt = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
        return issuedAt.AddHours(_securitySettings.EmailConfirmationTokenHours) < DateTimeOffset.UtcNow;
    }

    private bool IsResetRequestThrottled(User user)
    {
        if (string.IsNullOrWhiteSpace(user.PasswordResetToken) || user.PasswordResetTokenExpires == null)
            return false;

        var issuedAt = user.PasswordResetTokenExpires.Value
            .AddMinutes(-_securitySettings.PasswordResetTokenMinutes);

        var cooldownUntil = issuedAt.AddMinutes(_securitySettings.ResetRequestCooldownMinutes);
        return DateTimeOffset.UtcNow < cooldownUntil;
    }
}
