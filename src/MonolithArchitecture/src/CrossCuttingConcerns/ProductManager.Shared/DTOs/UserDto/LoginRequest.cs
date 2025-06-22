using System.ComponentModel.DataAnnotations;

namespace ProductManager.Shared.DTOs.UserDto;

public class LoginRequest : AccountFormModel
{
    public required string UserName { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public required string Password { get; set; } = string.Empty;
}
