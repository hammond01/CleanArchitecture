namespace ProductManager.Application.Feature.Identity.Commands;

public class UserRefreshTokenCommand : ICommand<ApiResponse>
{
    public UserRefreshTokenCommand(String accessToken, String refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
internal class UserRefreshTokenCommandHandler : ICommandHandler<UserRefreshTokenCommand, ApiResponse>
{
    private readonly IIdentityRepository _identityRepository;
    public UserRefreshTokenCommandHandler(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }
    public async Task<ApiResponse> HandleAsync(UserRefreshTokenCommand command, CancellationToken cancellationToken = default)
        => await _identityRepository.RefreshToken(command.AccessToken, command.RefreshToken);
}
