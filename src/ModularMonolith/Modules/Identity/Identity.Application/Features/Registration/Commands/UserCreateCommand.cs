using BuildingBlocks.Application.CQRS;

namespace Identity.Application.Features.Registration.Commands;

/// <summary>
/// Command to register a new user
/// </summary>
public record UserCreateCommand : ICommand<string>
{
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string ConfirmPassword { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
}
