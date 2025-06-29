// ReSharper disable All
namespace SolidTemplate.Constants.ConfigurationOptions;

public class IdentityConfig
{
    public static string ConfigName => "IdentityConfig";
    public string ISSUER { get; set; } = null!;
    public string AUDIENCE { get; set; } = null!;
    public string SECRET { get; set; } = null!;
    public TimeSpan BEARER_TOKEN_EXPIRATION { get; set; }
    public string BEARER_TOKEN_EXPIRATION_COMMENT { get; set; } = null!;
    public TimeSpan REFRESH_TOKEN_EXPIRATION { get; set; }
    public TimeSpan EMAIL_TOKEN_LIFETIME { get; set; }
    public TimeSpan PHONE_NUMBER_TOKEN_LIFETIME { get; set; }
    public TimeSpan RESET_PASSWORD_TOKEN_LIFETIME { get; set; }
    public TimeSpan TWO_FACTOR_TOKEN_LIFETIME { get; set; }
    public TimeSpan OTP_TOKEN_LIFETIME { get; set; }
    public TimeSpan REVOKE_USER_SESSIONS_DELAY { get; set; }
    public PasswordSettings PASSWORD { get; set; } = null!;
    public SignInSettings SIGN_IN { get; set; } = null!;
}
public class PasswordSettings
{
    public bool REQUIRE_DIGIT { get; set; }
    public int REQUIRED_LENGTH { get; set; }
    public bool REQUIRE_NON_ALPHANUMERIC { get; set; }
    public bool REQUIRE_UPPERCASE { get; set; }
    public bool REQUIRE_LOWERCASE { get; set; }
}
public class SignInSettings
{
    public bool REQUIRE_CONFIRMED_ACCOUNT { get; set; }
}
