using ProductManager.Constants.ApiResponseConstants;
using ProductManager.Domain.Entities.Identity;
using ProductManager.Shared.DTOs.AdminDto;
using ProductManager.Shared.DTOs.UserDto;
namespace ProductManager.Persistence.Repositories;

public class IdentityRepository : IIdentityRepository
{
    private readonly IDatabaseInitializer _databaseInitializer;
    private readonly EntityPermissions _entityPermissions;
    private readonly IdentityExtension _identityExtension;
    private readonly ILogger<IdentityRepository> _logger;
    private readonly RoleManager<Role> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    public IdentityRepository(IDatabaseInitializer databaseInitializer, EntityPermissions entityPermissions,
        ILogger<IdentityRepository> logger, RoleManager<Role> roleManager, UserManager<User> userManager,
        SignInManager<User> signInManager, IdentityExtension identityExtension)
    {
        _databaseInitializer = databaseInitializer;
        _entityPermissions = entityPermissions;
        _logger = logger;
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
        _identityExtension = identityExtension;
    }
    public async Task<ApiResponse> Login(LoginRequest parameters)
    {
        try
        {
            await _databaseInitializer.EnsureAdminIdentitiesAsync();

            var result = await _signInManager.PasswordSignInAsync(parameters.UserName, parameters.Password, parameters.RememberMe,
            true);

            if (result.RequiresTwoFactor)
            {
                _logger.LogInformation("Two factor authentication required for user " + parameters.UserName);

                return new ApiResponse(Status401Unauthorized, IdentityMessage.RequiresTwoFactor)
                {
                    Result = new LoginResponseModel
                    {
                        RequiresTwoFactor = true
                    }
                };
            }

            if (result.IsLockedOut)
            {
                _logger.LogInformation("User Locked out: " + parameters.UserName);
                return new ApiResponse(Status401Unauthorized, IdentityMessage.IsLockedOut);
            }

            if (result.IsNotAllowed)
            {
                _logger.LogInformation($"User {parameters.UserName} not allowed to log in, because email is not confirmed");
                return new ApiResponse(Status401Unauthorized, IdentityMessage.IsNotAllowed);
            }

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(parameters.UserName);
                _logger.LogInformation($"Logged In user {parameters.UserName}");

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user!.UserName!),
                    new Claim(ApplicationClaimTypes.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var userRoles = await _userManager.GetRolesAsync(user);
                authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
                authClaims.AddRange(userRoles.Select(userRole => new Claim(ApplicationClaimTypes.Role, userRole)));

                var roleQuery = _roleManager.Roles.AsQueryable().OrderBy(x => x.Name);
                var listResponse = roleQuery.ToList();
                var userPermissions = new List<string>();
                var roleDtoList = new List<RoleDto>();
                foreach (var role in listResponse)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    var permissions = claims.OrderBy(x => x.Value)
                        .Where(x => x.Type == ApplicationClaimTypes.Permission)
                        .Select(x => _entityPermissions.GetPermissionByValue(x.Value).Value).ToList();

                    roleDtoList.Add(new RoleDto
                    {
                        Name = role.Name!, Permissions = permissions
                    });
                }
                foreach (var role in userRoles)
                {
                    var roleDto = roleDtoList.FirstOrDefault(r => r.Name == role);
                    if (roleDto != null)
                    {
                        userPermissions.AddRange(roleDto.Permissions);
                    }
                }

                userPermissions = userPermissions.Distinct().ToList();

                authClaims.AddRange(userPermissions.Select(permission => new Claim(ApplicationClaimTypes.Permission, permission)));

                var token = _identityExtension.GenerateJwtToken(authClaims);
                var refreshToken = _identityExtension.GenerateRefreshToken();
                await _identityExtension.SaveRefreshTokenAsync(user.Id, refreshToken);
                var loginResponse = new LoginResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token), RefreshToken = refreshToken.Token
                };
                return new ApiResponse(Status200OK, IdentityMessage.LoginSuccess, loginResponse);
            }
            _logger.LogInformation($"Invalid Password for user {parameters.UserName}");
            return new ApiResponse(Status401Unauthorized, IdentityMessage.LoginFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login Failed: {ex.GetBaseException().Message}");
            return new ApiResponse(Status500InternalServerError, IdentityMessage.LoginFailed);
        }
    }
    public async Task<ApiResponse> RefreshToken(string token, string refreshToken)
    {
        var principal = _identityExtension.GetPrincipalFromExpiredToken(token);
        var userName = principal.Identity!.Name;

        var user = await _userManager.FindByNameAsync(userName!);
        if (user == null || !await _identityExtension.ValidateRefreshTokenAsync(user.Id, refreshToken))
        {
            return new ApiResponse(Status401Unauthorized, IdentityMessage.InvalidRefreshToken);
        }

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ApplicationClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userRoles = await _userManager.GetRolesAsync(user);
        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
        authClaims.AddRange(userRoles.Select(userRole => new Claim(ApplicationClaimTypes.Role, userRole)));

        var userClaims = await _userManager.GetClaimsAsync(user);
        authClaims.AddRange(userClaims);

        var roleQuery = _roleManager.Roles.AsQueryable().OrderBy(x => x.Name);
        var listResponse = roleQuery.ToList();
        var userPermissions = new List<string>();
        var roleDtoList = new List<RoleDto>();

        foreach (var role in listResponse)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            var permissions = claims.OrderBy(x => x.Value)
                .Where(x => x.Type == ApplicationClaimTypes.Permission)
                .Select(x => _entityPermissions.GetPermissionByValue(x.Value).Value).ToList();

            roleDtoList.Add(new RoleDto
            {
                Name = role.Name!, Permissions = permissions
            });
        }

        foreach (var role in userRoles)
        {
            var roleDto = roleDtoList.FirstOrDefault(r => r.Name == role);
            if (roleDto != null)
            {
                userPermissions.AddRange(roleDto.Permissions);
            }
        }

        authClaims.AddRange(userPermissions.Select(permission => new Claim(ApplicationClaimTypes.Permission, permission)));

        var newJwtToken = _identityExtension.GenerateJwtToken(authClaims);

        var newRefreshToken = _identityExtension.GenerateRefreshToken();
        await _identityExtension.SaveRefreshTokenAsync(user.Id, newRefreshToken);

        var response = new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(newJwtToken), RefreshToken = newRefreshToken.Token
        };

        // Revoked refresh tokens to be used
        await _identityExtension.RevokeRefreshTokenAsync(refreshToken);

        return new ApiResponse(Status200OK, IdentityMessage.TokenRefreshed, response);
    }
    public async Task<ApiResponse> Logout(ClaimsPrincipal authenticatedUser)
    {
        if (authenticatedUser.Identity!.IsAuthenticated)
        {
            await _signInManager.SignOutAsync();
        }
        return new ApiResponse(Status200OK, IdentityMessage.LogoutSuccess);
    }
    public async Task<ApiResponse> Register(RegisterRequest parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.UserName) ||
            string.IsNullOrWhiteSpace(parameters.Password))
        {
            return new ApiResponse(404, IdentityMessage.UserNameAndPassRequired);
        }

        var existingUser = await _userManager.FindByNameAsync(parameters.UserName);
        if (existingUser != null)
        {
            return new ApiResponse(404, IdentityMessage.UserAlreadyExists);
        }

        var newUser = new User
        {
            UserName = parameters.UserName,
            FirstName = parameters.FirstName,
            LastName = parameters.LastName,
            PhoneNumber = parameters.PhoneNumber
        };

        var result = await _userManager.CreateAsync(newUser, parameters.Password);
        if (result.Succeeded)
        {
            return new ApiResponse(200, IdentityMessage.RegisteredSuccess);
        }
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return new ApiResponse(404, $"{IdentityMessage.RegisteredFailed}{errors}");
    }
    public async Task<ApiResponse> ConfirmEmail(ConfirmEmailDto parameters)
    {
        var user = await _userManager.FindByIdAsync(parameters.UserId);

        if (user == null)
        {
            _logger.LogInformation($"The user {parameters.UserId} doesn't exist");
            return new ApiResponse(Status404NotFound, IdentityMessage.UserDoesNotExist);
        }

        if (user.EmailConfirmed)
        {
            return new ApiResponse(Status200OK, IdentityMessage.EmailVerificationSuccessful);
        }
        var token = parameters.Token;
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            var msg = result.GetErrors();
            return new ApiResponse(Status400BadRequest, msg);
        }

        await _userManager.RemoveClaimAsync(user,
        new Claim(ApplicationClaimTypes.EmailVerified, ClaimValues.FalseString, ClaimValueTypes.Boolean));
        await _userManager.AddClaimAsync(user,
        new Claim(ApplicationClaimTypes.EmailVerified, ClaimValues.TrueString, ClaimValueTypes.Boolean));

        return new ApiResponse(Status200OK, IdentityMessage.EmailVerificationSuccessful);
    }
}
