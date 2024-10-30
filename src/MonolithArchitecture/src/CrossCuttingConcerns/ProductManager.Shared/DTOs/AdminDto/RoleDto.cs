namespace ProductManager.Domain.DTOs.AdminDto;

public class RoleDto
{
    [StringLength(64, ErrorMessage = "ErrorInvalidLength", MinimumLength = 2)]
    [RegularExpression(@"[^\s]+", ErrorMessage = "SpacesNotPermitted")]
    [Display(Name = "Name")]
    public string Name { get; set; } = default!;
    public List<string> Permissions { get; set; } = default!;
}
