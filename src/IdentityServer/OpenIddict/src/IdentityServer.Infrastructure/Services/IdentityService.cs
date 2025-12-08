using IdentityServer.Domain.Contracts;
using IdentityServer.Domain.Entities;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result<Guid>> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = false
        };

        var exists = await _userManager.FindByNameAsync(request.UserName);
        if (exists != null)
            return Result<Guid>.Failure("Username already exists");

        var emailExists = await _userManager.FindByEmailAsync(request.Email);
        if (emailExists != null)
            return Result<Guid>.Failure("Email already in use");

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => e.Description).ToList();
            return Result<Guid>.Failure(errors);
        }

        return Result<Guid>.Success(user.Id);
    }

    public async Task<Result<string>> LoginAsync(LoginRequest request)
    {
        ApplicationUser? user = null;
        if (request.UserNameOrEmail.Contains("@"))
            user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
        else
            user = await _userManager.FindByNameAsync(request.UserNameOrEmail);

        if (user == null)
            return Result<string>.Failure("Invalid credentials");

        // Check if email is confirmed
        if (!user.EmailConfirmed)
            return Result<string>.Failure("Email not confirmed. Please confirm your email before logging in.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Result<string>.Failure("Account locked due to multiple failed login attempts. Please try again later.");

        if (result.IsNotAllowed)
            return Result<string>.Failure("Login not allowed. Please confirm your email or contact support.");

        if (result.RequiresTwoFactor)
            return Result<string>.Failure("Two-factor authentication required.");

        if (!result.Succeeded)
            return Result<string>.Failure("Invalid credentials");

        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Result<string>.Success(user.Id.ToString());
    }
}
