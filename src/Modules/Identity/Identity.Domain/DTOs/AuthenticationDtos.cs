namespace Identity.Domain.DTOs;

public record LoginRequestDto
{
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public bool RememberMe { get; init; }
}

public record LoginResponseDto
{
    public string Token { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public bool RequiresTwoFactor { get; init; }
}
