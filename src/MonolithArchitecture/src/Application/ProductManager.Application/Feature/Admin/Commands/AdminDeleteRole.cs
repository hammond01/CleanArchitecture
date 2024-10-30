namespace ProductManager.Application.Feature.Admin.Commands;

public class AdminDeleteRoleCommand : ICommand<ApiResponse>
{
    public AdminDeleteRoleCommand(string roleName)
    {
        RoleName = roleName;
    }
    public string RoleName { get; set; }
}
internal class AdminDeleteRoleCommandHandler : ICommandHandler<AdminDeleteRoleCommand, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminDeleteRoleCommandHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminDeleteRoleCommand command, CancellationToken cancellationToken = default)
        => await _adminRepository.AdminDeleteRoleAsync(command.RoleName);
}
