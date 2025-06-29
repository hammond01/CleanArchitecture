namespace ProductManager.Constants.AuthorizationDefinitions;

public static class ApplicationClaimTypes
{
    ///<summary>A claim that specifies the permission of an entity</summary>
    public const string Permission = "permission";
    public const string Role = "role";
    public const string Name = "name";
    public const string EmailVerified = "email_verified";
}
public static class ClaimValues
{
    public const string TrueString = "true";
    public const string FalseString = "false";

    public const string AuthenticationMethodMfa = "mfa";
    public const string AuthenticationMethodPwd = "pwd";
}
