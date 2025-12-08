using IdentityServer.Domain.Contracts;
using IdentityServer.Domain.Entities;
using IdentityServer.Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Commands;

/// <summary>
/// Command to request password reset
/// </summary>
public class ForgotPasswordCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Handler for forgot password
/// </summary>
public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ForgotPasswordHandler> _logger;

    public ForgotPasswordHandler(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        ILogger<ForgotPasswordHandler> logger)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async ValueTask<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                _logger.LogInformation("Password reset requested for non-existent email: {Email}", request.Email);
                return Result<string>.Success("If an account with that email exists, a password reset link has been sent.");
            }

            // Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning("Password reset requested for unconfirmed email: {Email}", request.Email);
                return Result<string>.Success("Please confirm your email first before resetting password.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = $"https://localhost:5001/Account/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            var emailData = new
            {
                FirstName = user.FirstName ?? "User",
                ResetUrl = resetUrl
            };

            await _emailSender.SendTemplatedEmailAsync(request.Email, "ResetPassword", emailData);

            _logger.LogInformation("Password reset email sent to {Email}", request.Email);
            return Result<string>.Success("Password reset link has been sent to your email.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process password reset for {Email}", request.Email);
            return Result<string>.Failure("Failed to process password reset request");
        }
    }
}

/// <summary>
/// Command to reset password
/// </summary>
public class ResetPasswordCommand : IRequest<Result<string>>
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

/// <summary>
/// Handler for resetting password
/// </summary>
public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResetPasswordHandler> _logger;

    public ResetPasswordHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<ResetPasswordHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async ValueTask<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found for password reset: {UserId}", request.UserId);
                return Result<string>.Failure("Invalid password reset request");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Password reset failed for {Email}: {Errors}", user.Email, errors);
                return Result<string>.Failure($"Password reset failed: {errors}");
            }

            _logger.LogInformation("Password reset successfully for {Email}", user.Email);
            return Result<string>.Success("Password has been reset successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset password for user {UserId}", request.UserId);
            return Result<string>.Failure("Failed to reset password");
        }
    }
}
