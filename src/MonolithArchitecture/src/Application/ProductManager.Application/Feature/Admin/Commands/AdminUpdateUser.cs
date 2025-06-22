using ProductManager.Application.Common.Commands;
using ProductManager.Domain.Common;
using ProductManager.Domain.Repositories;
using ProductManager.Shared.DTOs.UserDto;
namespace ProductManager.Application.Feature.Admin.Commands;

public class AdminUpdateUserCommand : ICommand<ApiResponse>
{
    public AdminUpdateUserCommand(UserDto request)
    {
        Request = request;
    }
    public UserDto Request { get; set; }
}
internal class AdminUpdateUserCommandHandler : ICommandHandler<AdminUpdateUserCommand, ApiResponse>
{
    private readonly IAdminRepository _adminRepository;
    public AdminUpdateUserCommandHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    public async Task<ApiResponse> HandleAsync(AdminUpdateUserCommand command, CancellationToken cancellationToken = default)
        => await _adminRepository.AdminUpdateUser(command.Request);
}
