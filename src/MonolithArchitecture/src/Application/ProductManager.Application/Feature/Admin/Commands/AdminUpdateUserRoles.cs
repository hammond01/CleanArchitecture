using ProductManager.Application.Common.Commands;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;
using ProductManager.Shared.DTOs.UserDto;

namespace ProductManager.Application.Feature.Admin.Commands;

public class AdminUpdateUserRolesCommand : ICommand<ApiResponse>
{
    public AdminUpdateUserRolesCommand(UpdateUserDto request)
    {
        Request = request;
    }
    public UpdateUserDto Request { get; set; }
}
internal class AdminUpdateUserRolesCommandHandler : ICommandHandler<AdminUpdateUserRolesCommand, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminUpdateUserRolesCommandHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminUpdateUserRolesCommand command, CancellationToken cancellationToken = default)
        => await _adminRepository.AdminUpdateRoleUser(command.Request);
}
