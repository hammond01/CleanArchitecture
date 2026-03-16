namespace Identity.Application.Services;

/// <summary>
/// Email payload for delivery
/// </summary>
public sealed class EmailMessage
{
    public string To { get; init; } = null!;
    public string Subject { get; init; } = null!;
    public string HtmlBody { get; init; } = null!;
    public string TextBody { get; init; } = null!;
}
