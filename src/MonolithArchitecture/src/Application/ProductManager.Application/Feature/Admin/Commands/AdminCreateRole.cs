namespace ProductManager.Application.Feature.Admin.Commands;

public class AdminCreateRoleCommand : ICommand<ApiResponse>
{
    public AdminCreateRoleCommand(RoleDto roleDto)
    {
        RoleDto = roleDto;
    }
    public RoleDto RoleDto { get; set; }
}
internal class AdminCreateRoleHandler : ICommandHandler<AdminCreateRoleCommand, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminCreateRoleHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminCreateRoleCommand command, CancellationToken cancellationToken = default)
        => await _adminRepository.AdminCreateRoleAsync(command.RoleDto);
}
