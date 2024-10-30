namespace ProductManager.Shared.DTOs.UserDto;

public class ChangePasswordDto
{
    public string UserId { get; set; } = default!;

    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;
    public string PasswordConfirm { get; set; } = default!;
}
