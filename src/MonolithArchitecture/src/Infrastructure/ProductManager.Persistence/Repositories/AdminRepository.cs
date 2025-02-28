using Microsoft.AspNetCore.Http;
using ProductManager.Domain.Entities.Identity;
using ProductManager.Shared.DTOs.AdminDto;
using ProductManager.Shared.DTOs.UserDto;
namespace ProductManager.Persistence.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly EntityPermissions _entityPermissions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AdminRepository> _logger;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public AdminRepository(EntityPermissions entityPermissions, ILogger<AdminRepository> logger,
        RoleManager<Role> roleManager,
        UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _entityPermissions = entityPermissions;
        _logger = logger;
        _roleManager = roleManager;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<ApiResponse> AdminUpdateUser(UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userDto.UserId.ToString());

        if (!user!.UserName!.Equals(DefaultUserNames.Administrator, StringComparison.CurrentCultureIgnoreCase) &&
            !userDto.UserName!.Equals(DefaultUserNames.Administrator, StringComparison.CurrentCultureIgnoreCase))
        {
            user.UserName = userDto.UserName;
        }

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.Email = userDto.Email;

        try
        {
            await _userManager.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Updating user exception: {ex.GetBaseException().Message}");
            return new ApiResponse(Status500InternalServerError, "Operation Failed");
        }

        if (userDto.Roles == null)
        {
            return new ApiResponse(Status200OK, "Operation Successful");
        }
        {
            try
            {
                var currentUserRoles = (List<string>)await _userManager.GetRolesAsync(user);

                var rolesToAdd = userDto.Roles.Where(newUserRole => !currentUserRoles.Contains(newUserRole)).ToList();

                if (rolesToAdd.Count > 0)
                {
                    await _userManager.AddToRolesAsync(user, rolesToAdd);

                    foreach (var role in rolesToAdd)
                    {
                        await _userManager.AddClaimAsync(user, new Claim($"Is{role}", ClaimValues.TrueString));
                    }
                }

                var rolesToRemove = currentUserRoles.Where(role => !userDto.Roles.Contains(role)).ToList();

                if (rolesToRemove.Count > 0)
                {
                    if (user.UserName.Equals(DefaultUserNames.Administrator, StringComparison.CurrentCultureIgnoreCase))
                    {
                        rolesToRemove.Remove(DefaultUserNames.Administrator);
                    }

                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                    foreach (var role in rolesToRemove)
                    {
                        await _userManager.RemoveClaimAsync(user, new Claim($"Is{role}", ClaimValues.TrueString));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Updating Roles exception: {ex.GetBaseException().Message}");
                return new ApiResponse(Status500InternalServerError, "Operation Failed");
            }
        }

        return new ApiResponse(Status200OK, "Operation Successful");
    }
    public async Task<ApiResponse> AdminResetUserPasswordAsync(ChangePasswordDto changePasswordDto,
        ClaimsPrincipal authenticatedUser)
    {
        var user = await _userManager.FindByIdAsync(changePasswordDto.UserId);
        if (user == null)
        {
            _logger.LogWarning($"The user {changePasswordDto.UserId} doesn't exist");
            return new ApiResponse(Status404NotFound, "The user doesn't exist");
        }
        var passToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, passToken, changePasswordDto.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation(user.UserName + "'s password reset; Requested from Admin interface by:" +
                                   authenticatedUser.Identity!.Name);
            return new ApiResponse(Status204NoContent, user.UserName + " password reset");
        }
        _logger.LogWarning(user.UserName + "'s password reset failed; Requested from Admin interface by:" +
                           authenticatedUser.Identity!.Name);

        var msg = result.GetErrors();
        _logger.LogWarning($"Error while resetting password of {user.UserName}: {msg}");
        return new ApiResponse(Status400BadRequest, msg);
    }
    public async Task<ApiResponse> AdminUpdateRoleUser(UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(updateUserDto.UserId.ToString());
            if (user == null)
            {
                return new ApiResponse(Status404NotFound, "User not found");
            }

            var currentUserRoles = (List<string>)await _userManager.GetRolesAsync(user);

            if (updateUserDto.Roles == null)
            {
                return new ApiResponse(Status200OK, "Operation Successful");
            }
            var rolesToAdd = updateUserDto.Roles.Where(newUserRole => !currentUserRoles.Contains(newUserRole)).ToList();

            if (rolesToAdd.Count > 0)
            {
                await _userManager.AddToRolesAsync(user, rolesToAdd);

                foreach (var role in rolesToAdd)
                {
                    await _userManager.AddClaimAsync(user, new Claim($"Is{role}", ClaimValues.TrueString));
                }
            }

            var rolesToRemove = currentUserRoles.Where(role => !updateUserDto.Roles.Contains(role)).ToList();

            if (rolesToRemove.Count <= 0)
            {
                return new ApiResponse(Status200OK, "Operation Successful");
            }
            {
                if (user.UserName!.Equals(DefaultUserNames.Administrator, StringComparison.CurrentCultureIgnoreCase))
                {
                    rolesToRemove.Remove(DefaultUserNames.Administrator);
                }

                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                foreach (var role in rolesToRemove)
                {
                    await _userManager.RemoveClaimAsync(user, new Claim($"Is{role}", ClaimValues.TrueString));
                }
            }
            return new ApiResponse(Status200OK, "Operation Successful");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Updating Roles exception: {ex.GetBaseException().Message}");
            return new ApiResponse(Status500InternalServerError, "Operation Failed");
        }
    }
    public async Task<ApiResponse> AdminGetUsers(int pageSize = 10, int pageNumber = 0)
    {
        var userList = _userManager.Users.AsQueryable();
        var count = userList.Count();
        var listResponse = userList.OrderBy(x => x.Id).Skip(pageNumber * pageSize).Take(pageSize).ToList();
        var userDtoList =
            new List<UserDto>();
        foreach (var applicationUser in listResponse)
        {
            userDtoList.Add(new UserDto
            {
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                UserName = applicationUser.UserName,
                Email = applicationUser.Email,
                UserId = applicationUser.Id,
                Roles = await _userManager.GetRolesAsync(applicationUser).ConfigureAwait(true) as List<string>
            });
        }
        return new ApiResponse(Status200OK, $"{count} users fetched", userDtoList);
    }
    public ApiResponse AdminGetPermissions()
    {
        var permissions = _entityPermissions.GetAllPermissionNames();
        return new ApiResponse(Status200OK, "List permission fetched", permissions);
    }
    public async Task<ApiResponse> AdminGetRolesAsync(int pageSize = 0, int pageNumber = 0)
    {
        var roleQuery = _roleManager.Roles.AsQueryable().OrderBy(x => x.Name);
        var count = roleQuery.Count();
        var listResponse = (pageSize > 0 ? roleQuery.Skip(pageNumber * pageSize).Take(pageSize) : roleQuery).ToList();

        var roleDtoList = new List<RoleDto>();

        foreach (var role in listResponse)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            var permissions = claims.OrderBy(x => x.Value).Where(x => x.Type == ApplicationClaimTypes.Permission)
                .Select(x => _entityPermissions.GetPermissionByValue(x.Value).Name).ToList();

            roleDtoList.Add(new RoleDto
            {
                Name = role.Name ?? "", Permissions = permissions
            });
        }
        return new ApiResponse(Status200OK, $"{count} roles fetched", roleDtoList);
    }
    public async Task<ApiResponse> AdminGetRoleAsync(string name)
    {
        var identityRole = await _roleManager.FindByNameAsync(name);

        var claims = await _roleManager.GetClaimsAsync(identityRole!);
        var permissions = claims.OrderBy(x => x.Value).Where(x => x.Type == ApplicationClaimTypes.Permission)
            .Select(x => _entityPermissions.GetPermissionByValue(x.Value).Name).ToList();

        var roleDto = new RoleDto
        {
            Name = name, Permissions = permissions
        };

        return new ApiResponse(Status200OK, "List roles fetched", roleDto);
    }
    public async Task<ApiResponse> AdminCreateRoleAsync(RoleDto roleDto)
    {
        if (_roleManager.Roles.Any(r => r.Name == roleDto.Name))
        {
            return new ApiResponse(Status400BadRequest, "Role already exists");
        }

        var result = await _roleManager.CreateAsync(new Role(roleDto.Name));

        if (!result.Succeeded)
        {
            var msg = result.GetErrors();
            return new ApiResponse(Status400BadRequest, msg);
        }

        var role = await _roleManager.FindByNameAsync(roleDto.Name);

        foreach (var claim in roleDto.Permissions)
        {
            var resultAddClaim = await _roleManager.AddClaimAsync(role!,
            new Claim(ApplicationClaimTypes.Permission, _entityPermissions.GetPermissionByName(claim)));

            if (!resultAddClaim.Succeeded)
            {
                await _roleManager.DeleteAsync(role!);
            }
        }
        return new ApiResponse(Status200OK, "Role created");
    }
    public async Task<ApiResponse> AdminUpdateRoleAsync(RoleDto roleDto)
    {

        if (!_roleManager.Roles.Any(r => r.Name == roleDto.Name))
        {
            return new ApiResponse(Status400BadRequest, $"Role {roleDto.Name} not fount");
        }
        if (roleDto.Name == DefaultRoleNames.Administrator)
        {
            return new ApiResponse(Status403Forbidden, $"Role {roleDto.Name} cannot be edited");
        }
        // Create the permissions
        var role = await _roleManager.FindByNameAsync(roleDto.Name);
        if (role == null)
        {
            return new ApiResponse(Status400BadRequest, $"Role {roleDto.Name} not fount");
        }

        var claims = await _roleManager.GetClaimsAsync(role);
        var permissions = claims.OrderBy(x => x.Value).Where(x => x.Type == ApplicationClaimTypes.Permission)
            .Select(x => x.Value).ToList();
        foreach (var permission in permissions)
        {
            await _roleManager.RemoveClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission));
        }
        foreach (var claim in roleDto.Permissions)
        {
            var result = await _roleManager.AddClaimAsync(role,
            new Claim(ApplicationClaimTypes.Permission, _entityPermissions.GetPermissionByName(claim)));

            if (!result.Succeeded)
            {
                await _roleManager.DeleteAsync(role);
            }
        }
        return new ApiResponse(200, $"Role {roleDto.Name} updated", roleDto);
    }
    public async Task<ApiResponse> AdminDeleteRoleAsync(string name)
    {

        var users = await _userManager.GetUsersInRoleAsync(name);
        if (users.Any())
        {
            return new ApiResponse(Status404NotFound, $"Role {name} In use warning");
        }
        if (name == DefaultRoleNames.Administrator)
        {
            return new ApiResponse(Status403Forbidden, $"Role {name} cannot be deleted");
        }
        // Delete the role
        var role = await _roleManager.FindByNameAsync(name);
        await _roleManager.DeleteAsync(role!);

        return new ApiResponse(200, $"Role {name} deleted");
    }
    public async Task<ApiResponse> AdminUpdateUserRoles(UpdateUserDto updateUserDto)
    {
        var user = await _userManager.FindByIdAsync(updateUserDto.UserId.ToString());
        if (user == null)
        {
            return new ApiResponse(Status404NotFound, "User not found");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, roles);
        if (!result.Succeeded)
        {
            return new ApiResponse(Status400BadRequest, result.GetErrors());
        }

        result = await _userManager.AddToRolesAsync(user, updateUserDto.Roles!);
        return !result.Succeeded
            ? new ApiResponse(Status400BadRequest, result.GetErrors())
            : new ApiResponse(Status200OK, "User roles updated");
    }
    public string GetUserLogin()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return "";
        }
        var userName = _httpContextAccessor.HttpContext!.User.Identity!.Name!;
        return string.IsNullOrEmpty(userName) ? "" : userName.ToUpper();
    }
}
