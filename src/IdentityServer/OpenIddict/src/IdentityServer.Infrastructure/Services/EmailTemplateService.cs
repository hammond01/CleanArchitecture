using IdentityServer.Domain.Contracts;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Infrastructure.Services;

/// <summary>
/// Email template service implementation
/// </summary>
public class EmailTemplateService : IEmailTemplateService
{
    private readonly ILogger<EmailTemplateService> _logger;

    public EmailTemplateService(ILogger<EmailTemplateService> logger)
    {
        _logger = logger;
    }

    public async Task<string> RenderTemplateAsync(string templateName, object data)
    {
        try
        {
            // For now, we'll use embedded templates
            // In production, you might load templates from files or database
            var template = await LoadTemplateAsync(templateName);
            var rendered = await RenderTemplate(template, data);

            _logger.LogInformation("Template {TemplateName} rendered successfully", templateName);
            return rendered;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to render template {TemplateName}", templateName);
            throw;
        }
    }

    private async Task<string> LoadTemplateAsync(string templateName)
    {
        // Embedded templates - in production, load from files
        return templateName switch
        {
            "Welcome" => await Task.FromResult(@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Welcome to Identity Server</title>
                    <style>
                        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: #007bff; color: white; padding: 20px; text-align: center; }
                        .content { padding: 20px; background: #f9f9f9; }
                        .button { display: inline-block; padding: 10px 20px; background: #28a745; color: white; text-decoration: none; border-radius: 5px; }
                        .footer { text-align: center; padding: 20px; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Welcome to Identity Server</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {FirstName}!</h2>
                            <p>Your account has been created successfully.</p>
                            <p>Please confirm your email address to get started.</p>
                            <p><a href='{ConfirmUrl}' class='button'>Confirm Email</a></p>
                            <p>If the button doesn't work, copy and paste this link: {ConfirmUrl}</p>
                        </div>
                        <div class='footer'>
                            <p>This email was sent by Identity Server. If you didn't create an account, please ignore this email.</p>
                        </div>
                    </div>
                </body>
                </html>
            "),

            "ConfirmEmail" => await Task.FromResult(@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Confirm Your Email</title>
                    <style>
                        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: #007bff; color: white; padding: 20px; text-align: center; }
                        .content { padding: 20px; background: #f9f9f9; }
                        .button { display: inline-block; padding: 10px 20px; background: #28a745; color: white; text-decoration: none; border-radius: 5px; }
                        .footer { text-align: center; padding: 20px; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Confirm Your Email Address</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {FirstName}!</h2>
                            <p>Please confirm your email address to complete your registration.</p>
                            <p><a href='{ConfirmUrl}' class='button'>Confirm Email</a></p>
                            <p>If the button doesn't work, copy and paste this link: {ConfirmUrl}</p>
                            <p>This link will expire in 24 hours.</p>
                        </div>
                        <div class='footer'>
                            <p>This email was sent by Identity Server. If you didn't create an account, please ignore this email.</p>
                        </div>
                    </div>
                </body>
                </html>
            "),

            "ResetPassword" => await Task.FromResult(@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Reset Your Password</title>
                    <style>
                        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: #dc3545; color: white; padding: 20px; text-align: center; }
                        .content { padding: 20px; background: #f9f9f9; }
                        .button { display: inline-block; padding: 10px 20px; background: #dc3545; color: white; text-decoration: none; border-radius: 5px; }
                        .footer { text-align: center; padding: 20px; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Reset Your Password</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {FirstName}!</h2>
                            <p>You have requested to reset your password.</p>
                            <p><a href='{ResetUrl}' class='button'>Reset Password</a></p>
                            <p>If the button doesn't work, copy and paste this link: {ResetUrl}</p>
                            <p>This link will expire in 1 hour.</p>
                            <p>If you didn't request this password reset, please ignore this email.</p>
                        </div>
                        <div class='footer'>
                            <p>This email was sent by Identity Server.</p>
                        </div>
                    </div>
                </body>
                </html>
            "),

            "SecurityAlert" => await Task.FromResult(@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Security Alert</title>
                    <style>
                        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: #ffc107; color: #333; padding: 20px; text-align: center; }
                        .content { padding: 20px; background: #f9f9f9; }
                        .footer { text-align: center; padding: 20px; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Security Alert</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {FirstName}!</h2>
                            <p>We detected a security event on your account:</p>
                            <p><strong>{Event}</strong></p>
                            <p>Time: {Timestamp}</p>
                            <p>IP Address: {IPAddress}</p>
                            <p>If this was you, no action is needed.</p>
                            <p>If you don't recognize this activity, please change your password immediately.</p>
                        </div>
                        <div class='footer'>
                            <p>This email was sent by Identity Server.</p>
                        </div>
                    </div>
                </body>
                </html>
            "),

            "TwoFactorCode" => await Task.FromResult(@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Your 2FA Code</title>
                    <style>
                        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: #6f42c1; color: white; padding: 20px; text-align: center; }
                        .content { padding: 20px; background: #f9f9f9; }
                        .code { font-size: 24px; font-weight: bold; text-align: center; padding: 20px; background: white; border: 2px solid #6f42c1; border-radius: 5px; margin: 20px 0; }
                        .footer { text-align: center; padding: 20px; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Your Two-Factor Code</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {FirstName}!</h2>
                            <p>Your two-factor authentication code is:</p>
                            <div class='code'>{Code}</div>
                            <p>This code will expire in 5 minutes.</p>
                            <p>Do not share this code with anyone.</p>
                        </div>
                        <div class='footer'>
                            <p>This email was sent by Identity Server.</p>
                        </div>
                    </div>
                </body>
                </html>
            "),

            _ => await Task.FromResult("<p>This is a notification from Identity Server.</p>")
        };
    }

    private async Task<string> RenderTemplate(string template, object data)
    {
        var result = template;

        // Simple property replacement
        var properties = data.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(data)?.ToString() ?? string.Empty;
            result = result.Replace($"{{{prop.Name}}}", value);
        }

        return await Task.FromResult(result);
    }
}
