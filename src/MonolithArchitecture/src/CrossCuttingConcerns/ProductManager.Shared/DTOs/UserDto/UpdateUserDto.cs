namespace ProductManager.Shared.DTOs.UserDto;

public class UpdateUserDto
{
    public Guid UserId { get; set; }
    public List<string>? Roles { get; set; }
}
