using BuildingBlocks.Application.CQRS;

namespace Identity.Application.Features.PasswordManagement.Commands;

/// <summary>
/// Command to reset user password
/// </summary>
public record ResetPasswordCommand : ICommand<bool>
{
    public string UserId { get; init; } = null!;
    public string Token { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string ConfirmPassword { get; init; } = null!;
}
