using Identity.Application.Services;
using Identity.Domain.Entities;

namespace Identity.Infrastructure.Services;

/// <summary>
/// Builds email messages from templates
/// </summary>
public interface IEmailTemplateProvider
{
    EmailMessage BuildConfirmationEmail(User user, string token, string baseUrl);
    EmailMessage BuildWelcomeEmail(User user, string baseUrl);
    EmailMessage BuildPasswordResetEmail(User user, string token, string baseUrl);
    EmailMessage BuildPasswordChangedEmail(User user, string baseUrl);
}
