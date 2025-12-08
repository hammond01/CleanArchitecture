using IdentityServer.Domain.Contracts;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IdentityServer.Infrastructure.Services;

/// <summary>
/// Email configuration settings
/// </summary>
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

/// <summary>
/// Email sender implementation using MailKit
/// </summary>
public class EmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<EmailSettings> settings, ILogger<EmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlMessage, string? textMessage = null)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder();

            if (!string.IsNullOrEmpty(textMessage))
            {
                builder.TextBody = textMessage;
            }

            builder.HtmlBody = htmlMessage;
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();

            // For demo purposes, we'll use a simple SMTP configuration
            // In production, you should use proper SMTP settings
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort,
                _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

            if (!string.IsNullOrEmpty(_settings.SmtpUsername))
            {
                await client.AuthenticateAsync(_settings.SmtpUsername, _settings.SmtpPassword);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            throw;
        }
    }

    public async Task SendTemplatedEmailAsync(string to, string templateName, object templateData)
    {
        // For now, we'll use a simple template rendering
        // In a real application, you might use Razor templates or a template engine
        var subject = GetTemplateSubject(templateName);
        var htmlContent = await RenderSimpleTemplate(templateName, templateData);

        await SendEmailAsync(to, subject, htmlContent);
    }

    private string GetTemplateSubject(string templateName)
    {
        return templateName switch
        {
            "Welcome" => "Welcome to Identity Server",
            "ConfirmEmail" => "Please confirm your email address",
            "ResetPassword" => "Reset your password",
            "SecurityAlert" => "Security Alert",
            "TwoFactorCode" => "Your two-factor authentication code",
            _ => "Identity Server Notification"
        };
    }

    private Task<string> RenderSimpleTemplate(string templateName, object data)
    {
        // Simple template rendering - in production, use a proper template engine
        var template = templateName switch
        {
            "ConfirmEmail" => @"
                <h2>Please confirm your email address</h2>
                <p>Click the link below to confirm your email:</p>
                <a href='{ConfirmUrl}'>Confirm Email</a>
                <p>This link will expire in 24 hours.</p>
            ",
            "ResetPassword" => @"
                <h2>Reset your password</h2>
                <p>Click the link below to reset your password:</p>
                <a href='{ResetUrl}'>Reset Password</a>
                <p>This link will expire in 1 hour.</p>
            ",
            "Welcome" => @"
                <h2>Welcome to Identity Server!</h2>
                <p>Your account has been created successfully.</p>
                <p>Please confirm your email to get started.</p>
            ",
            _ => "<p>This is a notification from Identity Server.</p>"
        };

        // Simple property replacement
        var properties = data.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(data)?.ToString() ?? string.Empty;
            template = template.Replace($"{{{prop.Name}}}", value);
        }

        return Task.FromResult(template);
    }
}
