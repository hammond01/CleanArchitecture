using Identity.Application.Services;
using Identity.Domain.Entities;

namespace Identity.Infrastructure.Services;

/// <summary>
/// Simple string-based email templates
/// </summary>
public class EmailTemplateProvider : IEmailTemplateProvider
{
    public EmailMessage BuildConfirmationEmail(User user, string token, string baseUrl)
    {
        var link = BuildLink(baseUrl, "confirm-email", user.Id, token);
        var subject = "Confirm your email";
        var htmlBody = $@"
            <html>
            <body>
                <p>Hi {user.FirstName},</p>

                <p>Please confirm your email by clicking the link below:</p>

                <p>
                    <a href=""{link}""
                    style=""background-color:#2563eb;
                            color:white;
                            padding:10px 16px;
                            text-decoration:none;
                            border-radius:6px;"">
                        Confirm Email
                    </a>
                </p>

                <p>If you did not create this account, you can ignore this email.</p>
            </body>
            </html>";
        var textBody = $"Hi {user.FirstName},\n\nConfirm your email: {link}\n\nIf you did not create this account, ignore this email.";

        return new EmailMessage
        {
            To = user.Email,
            Subject = subject,
            HtmlBody = htmlBody,
            TextBody = textBody
        };
    }

    public EmailMessage BuildWelcomeEmail(User user, string baseUrl)
    {
        var subject = "Welcome to CleanArchitecture";
        var htmlBody = $@"<p>Hi {user.FirstName},</p>
<p>Your email has been confirmed. Welcome aboard!</p>
<p>You can sign in at: {baseUrl}</p>";
        var textBody = $"Hi {user.FirstName},\n\nYour email has been confirmed. Welcome aboard!\nSign in at: {baseUrl}";

        return new EmailMessage
        {
            To = user.Email,
            Subject = subject,
            HtmlBody = htmlBody,
            TextBody = textBody
        };
    }

    public EmailMessage BuildPasswordResetEmail(User user, string token, string baseUrl)
    {
        var link = BuildLink(baseUrl, "reset-password", user.Id, token);
        var subject = "Reset your password";
        var htmlBody = $@"
                <html>
                <body>
                    <p>Hi {user.FirstName},</p>

                    <p>You requested a password reset. Click below to set a new password:</p>

                    <p>
                        <a href=""{link}""
                        style=""background:#2563eb;
                                color:#fff;
                                padding:12px 18px;
                                text-decoration:none;
                                border-radius:6px;
                                display:inline-block;"">
                            Reset Password
                        </a>
                    </p>

                    <p>If you did not request this, you can ignore this email.</p>
                </body>
                </html>";
        var textBody = $"Hi {user.FirstName},\n\nReset your password: {link}\n\nIf you did not request this, ignore this email.";

        return new EmailMessage
        {
            To = user.Email,
            Subject = subject,
            HtmlBody = htmlBody,
            TextBody = textBody
        };
    }

    public EmailMessage BuildPasswordChangedEmail(User user, string baseUrl)
    {
        var subject = "Your password was changed";
        var htmlBody = $@"<p>Hi {user.FirstName},</p>
<p>Your password has been changed successfully.</p>
<p>If this was not you, please reset your password immediately.</p>";
        var textBody = $"Hi {user.FirstName},\n\nYour password has been changed successfully.\nIf this was not you, reset your password immediately.";

        return new EmailMessage
        {
            To = user.Email,
            Subject = subject,
            HtmlBody = htmlBody,
            TextBody = textBody
        };
    }

    private static string BuildLink(string baseUrl, string route, Guid userId, string token)
    {
        var trimmed = baseUrl.TrimEnd('/');
        var encodedToken = Uri.EscapeDataString(token);
        return $"{trimmed}/{route}?userId={userId}&token={encodedToken}";
    }
}
