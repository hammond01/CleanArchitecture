namespace Identity.Domain.DTOs;

public record RegisterRequestDto
{
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string ConfirmPassword { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
}

public record ConfirmEmailDto
{
    public string UserId { get; init; } = null!;
    public string Token { get; init; } = null!;
}
