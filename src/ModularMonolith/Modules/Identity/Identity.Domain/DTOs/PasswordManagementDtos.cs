namespace Identity.Domain.DTOs;

public record ResetPasswordRequestDto
{
    public string UserId { get; init; } = null!;
    public string Token { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string ConfirmPassword { get; init; } = null!;
}
