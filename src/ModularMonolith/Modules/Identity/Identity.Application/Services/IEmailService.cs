namespace Identity.Application.Services;

/// <summary>
/// Email sending abstraction
/// </summary>
public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
