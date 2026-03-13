namespace CleanArchitecture.Api.Configuration;

public sealed class ApiSecurityOptions
{
    public const string SectionName = "ApiSecurity";

    public string[] AllowedOrigins { get; init; } = [];

    public string[] KnownProxies { get; init; } = [];

    public bool AllowCredentials { get; init; }
}
