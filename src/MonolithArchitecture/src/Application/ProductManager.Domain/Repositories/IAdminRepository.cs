using ProductManager.Domain.DTOs.AdminDto;
namespace ProductManager.Domain.Repositories;

public interface IAdminRepository
{
    Task<ApiResponse> AdminUpdateUser(UserDto userDto);
    Task<ApiResponse> AdminResetUserPasswordAsync(ChangePasswordDto changePasswordDto, ClaimsPrincipal authenticatedUser);
    Task<ApiResponse> AdminUpdateRoleUser(UpdateUserDto updateUserDto);
    Task<ApiResponse> AdminGetUsers(int pageSize = 10, int pageNumber = 0);
    ApiResponse AdminGetPermissions();
    Task<ApiResponse> AdminGetRolesAsync(int pageSize = 0, int pageNumber = 0);
    Task<ApiResponse> AdminGetRoleAsync(string name);
    Task<ApiResponse> AdminCreateRoleAsync(RoleDto roleDto);
    Task<ApiResponse> AdminUpdateRoleAsync(RoleDto roleDto);
    Task<ApiResponse> AdminDeleteRoleAsync(string name);
    Task<ApiResponse> AdminUpdateUserRoles(UpdateUserDto updateUserDto);
    string GetUserLogin();
}
