namespace CleanArchitecture.Api.Configuration;

public sealed class AuthRateLimitOptions
{
    public const string SectionName = "RateLimiting:AuthEndpoints";

    public int PermitLimit { get; init; } = 30;

    public int WindowSeconds { get; init; } = 60;

    public int QueueLimit { get; init; }
}
