namespace ProductManager.Shared.DTOs.UserDto;

public class RefreshTokenDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
