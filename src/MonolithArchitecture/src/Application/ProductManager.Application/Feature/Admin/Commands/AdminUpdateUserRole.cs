namespace ProductManager.Application.Feature.Admin.Commands;

public class AdminUpdateUserRoleCommand : ICommand<ApiResponse>
{
    public AdminUpdateUserRoleCommand(UpdateUserDto request)
    {
        Request = request;
    }
    public UpdateUserDto Request { get; set; }
}
internal class AdminUpdateUserRoleCommandHandler : ICommandHandler<AdminUpdateUserRoleCommand, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminUpdateUserRoleCommandHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminUpdateUserRoleCommand command, CancellationToken cancellationToken = default)
        => await _adminRepository.AdminUpdateRoleUser(command.Request);
}
