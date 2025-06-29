namespace ProductManager.Constants.ApiResponseConstants;

public static class IdentityMessage
{
    public static string EmailVerificationSuccessful => "Email verification successful";
    public static string InvalidRefreshToken => "Invalid refresh token";
    public static string IsLockedOut => "User is locked out";
    public static string IsNotAllowed => "Email not confirmed";
    public static string LoginFailed => "Login failed";
    public static string LoginSuccess => "Login success";
    public static string LogoutSuccess => "Logout success";
    public static string RegisteredFailed => "User registration failed: ";
    public static string RegisteredSuccess => "User registered successfully.";
    public static string RequiresTwoFactor => "Two factor authentication required";
    public static string TokenRefreshed => "Token refreshed";
    public static string UserAlreadyExists => "User already exists";
    public static string UserDoesNotExist => "The user doesn't exist.";
    public static string UserNameAndPassRequired => "Username and password are required.";
}
