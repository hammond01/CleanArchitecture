namespace ProductManager.Application.Feature.Admin.Commands;

public class AdminResetUserPasswordCommand : ICommand<ApiResponse>
{
    public AdminResetUserPasswordCommand(ChangePasswordDto request, ClaimsPrincipal useClaimsPrincipal)
    {
        Request = request;
        UseClaimsPrincipal = useClaimsPrincipal;
    }
    public ChangePasswordDto Request { get; set; }
    public ClaimsPrincipal UseClaimsPrincipal { get; set; }
}
internal class AdminResetUserPasswordCommandHandler : ICommandHandler<AdminResetUserPasswordCommand, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminResetUserPasswordCommandHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminResetUserPasswordCommand command,
        CancellationToken cancellationToken = default)
        => await _adminRepository.AdminResetUserPasswordAsync(command.Request, command.UseClaimsPrincipal);
}
