namespace ProductManager.Domain.Entities;

public class User : IdentityUser<string>
{
    [StringLength(250)]
    public string Auth0UserId { get; set; } = string.Empty;
    [StringLength(250)]
    public string AzureAdB2CUserId { get; set; } = string.Empty;
}
