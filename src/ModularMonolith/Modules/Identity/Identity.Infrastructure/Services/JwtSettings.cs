namespace Identity.Infrastructure.Services;

/// <summary>
/// JWT runtime settings for token generation and validation
/// </summary>
public sealed class JwtSettings
{
    public string Issuer { get; set; } = "CleanArchitecture";
    public string Audience { get; set; } = "CleanArchitecture.Client";
    public string SecretKey { get; set; } = "CleanArchitecture_Dev_Secret_Key_1234567890_ChangeMe";
    public int AccessTokenMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 7;
    public int RememberMeRefreshTokenDays { get; set; } = 30;
}
