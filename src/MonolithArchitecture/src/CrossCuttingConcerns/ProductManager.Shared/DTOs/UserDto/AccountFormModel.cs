namespace ProductManager.Shared.DTOs.UserDto;

public class AccountFormModel
{
    private string _returnUrl = null!;

    public bool RememberMe { get; set; }

    public string ReturnUrl
    {
        get => _returnUrl ?? "/";
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.StartsWith("http"))
                {
                    value = new Uri(value).LocalPath;
                }

                if (!value.StartsWith("/"))
                {
                    value = $"/{value}";
                }
            }

            _returnUrl = value;
        }
    }

    public string? RequestVerificationToken { get; set; }
}
