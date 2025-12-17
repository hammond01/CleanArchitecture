using System.ComponentModel.DataAnnotations;

namespace ProductManager.Shared.DTOs.UserDto;

public class ResetPasswordRequestDto
{
    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;
}
