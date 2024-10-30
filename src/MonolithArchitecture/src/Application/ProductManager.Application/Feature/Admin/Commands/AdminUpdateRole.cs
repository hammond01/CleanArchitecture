namespace ProductManager.Application.Feature.Admin.Commands;

public class AdminUpdateRoleCommand : ICommand<ApiResponse>
{
    public AdminUpdateRoleCommand(RoleDto request)
    {
        Request = request;
    }
    public RoleDto Request { get; set; }
}
internal class AdminUpdateRoleCommandHandler : ICommandHandler<AdminUpdateRoleCommand, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminUpdateRoleCommandHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminUpdateRoleCommand command, CancellationToken cancellationToken = default)
        => await _adminRepository.AdminUpdateRoleAsync(command.Request);
}
