using System.Collections.Concurrent;
using Identity.Application.Services;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Services;

/// <summary>
/// Email service that captures messages for testing/dev
/// </summary>
public class FakeEmailService : IEmailService
{
    private static readonly ConcurrentQueue<EmailMessage> Messages = new();
    private readonly ILogger<FakeEmailService> _logger;

    public FakeEmailService(ILogger<FakeEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        Messages.Enqueue(message);
        _logger.LogInformation("Fake email sent to {To} with subject '{Subject}'", message.To, message.Subject);
        return Task.CompletedTask;
    }

    public static EmailMessage? GetLastMessage(string? recipient = null)
    {
        var messages = Messages.ToArray().AsEnumerable();
        if (!string.IsNullOrWhiteSpace(recipient))
        {
            messages = messages.Where(message =>
                string.Equals(message.To, recipient, StringComparison.OrdinalIgnoreCase));
        }

        return messages.LastOrDefault();
    }

    public static void Clear()
    {
        while (Messages.TryDequeue(out _))
        {
        }
    }
}
