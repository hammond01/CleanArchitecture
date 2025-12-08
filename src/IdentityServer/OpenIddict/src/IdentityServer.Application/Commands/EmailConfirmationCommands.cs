using IdentityServer.Domain.Contracts;
using IdentityServer.Domain.Entities;
using IdentityServer.Domain.Common;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Commands;

/// <summary>
/// Command to send email confirmation
/// </summary>
public class SendConfirmationEmailCommand : IRequest<Result<string>>
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
}

/// <summary>
/// Handler for sending confirmation email
/// </summary>
public class SendConfirmationEmailHandler : IRequestHandler<SendConfirmationEmailCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<SendConfirmationEmailHandler> _logger;

    public SendConfirmationEmailHandler(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        ILogger<SendConfirmationEmailHandler> logger)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async ValueTask<Result<string>> Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found for email confirmation: {UserId}", request.UserId);
                return Result<string>.Failure("User not found");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmUrl = $"https://localhost:5001/Account/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            var emailData = new
            {
                FirstName = request.FirstName,
                ConfirmUrl = confirmUrl
            };

            await _emailSender.SendTemplatedEmailAsync(request.Email, "ConfirmEmail", emailData);

            _logger.LogInformation("Confirmation email sent to {Email}", request.Email);
            return Result<string>.Success("Confirmation email sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to {Email}", request.Email);
            return Result<string>.Failure("Failed to send confirmation email");
        }
    }
}

/// <summary>
/// Command to confirm email
/// </summary>
public class ConfirmEmailCommand : IRequest<Result<string>>
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Handler for confirming email
/// </summary>
public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ConfirmEmailHandler> _logger;

    public ConfirmEmailHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<ConfirmEmailHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async ValueTask<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found for email confirmation: {UserId}", request.UserId);
                return Result<string>.Failure("User not found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Email confirmation failed for {Email}: {Errors}", user.Email, errors);
                return Result<string>.Failure($"Email confirmation failed: {errors}");
            }

            _logger.LogInformation("Email confirmed successfully for {Email}", user.Email);
            return Result<string>.Success("Email confirmed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to confirm email for user {UserId}", request.UserId);
            return Result<string>.Failure("Failed to confirm email");
        }
    }
}
