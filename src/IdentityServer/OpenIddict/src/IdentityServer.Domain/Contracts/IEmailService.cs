using System.Net.Mail;

namespace IdentityServer.Domain.Contracts;

/// <summary>
/// Email sending service interface
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Send an email asynchronously
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="htmlMessage">HTML message content</param>
    /// <param name="textMessage">Plain text message content (optional)</param>
    /// <returns>Task representing the async operation</returns>
    Task SendEmailAsync(string to, string subject, string htmlMessage, string? textMessage = null);

    /// <summary>
    /// Send a templated email asynchronously
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="templateName">Template name (e.g., "Welcome", "ConfirmEmail")</param>
    /// <param name="templateData">Data to populate the template</param>
    /// <returns>Task representing the async operation</returns>
    Task SendTemplatedEmailAsync(string to, string templateName, object templateData);
}

/// <summary>
/// Email template service interface
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Render an email template with data
    /// </summary>
    /// <param name="templateName">Template name</param>
    /// <param name="data">Template data</param>
    /// <returns>Rendered HTML content</returns>
    Task<string> RenderTemplateAsync(string templateName, object data);
}
