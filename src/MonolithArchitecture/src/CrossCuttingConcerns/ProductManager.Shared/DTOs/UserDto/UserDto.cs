namespace ProductManager.Shared.DTOs.UserDto;

public class UserDto : BaseDto
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public List<string>? Roles { get; set; }
}
