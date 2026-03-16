namespace Identity.Infrastructure.Services;

/// <summary>
/// Email configuration settings
/// </summary>
public sealed class EmailSettings
{
    public bool UseFakeEmail { get; set; } = true;
    public string FromName { get; set; } = "CleanArchitecture";
    public string FromEmail { get; set; } = "no-reply@cleanarchitecture.local";
    public string BaseUrl { get; set; } = "http://localhost";

    public string SmtpHost { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 25;
    public bool SmtpUseSsl { get; set; } = false;
    public string? SmtpUser { get; set; }
    public string? SmtpPass { get; set; }
}
