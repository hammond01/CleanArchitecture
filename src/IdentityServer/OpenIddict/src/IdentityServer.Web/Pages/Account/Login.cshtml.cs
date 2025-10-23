using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer.Domain.Entities;

namespace IdentityServer.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

    public class InputModel
    {
        [Required(ErrorMessage = "Email or Username is required")]
        [Display(Name = "Email or Username")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            Response.Redirect(returnUrl ?? "/");
            return;
        }

        ReturnUrl = returnUrl;

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        // Get external login providers
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }

        try
        {
            // Find user by email or username
            var user = await FindUserAsync(Input.UserNameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                _logger.LogWarning("Login failed: User not found for '{UserNameOrEmail}'", Input.UserNameOrEmail);
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            // Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Email not confirmed. Please check your email.");
                _logger.LogWarning("Login failed: Email not confirmed for user '{UserId}'", user.Id);
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            // Attempt to sign in
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User '{UserName}' logged in successfully", user.UserName);

                // Redirect to admin dashboard if user is admin, otherwise to return URL
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return LocalRedirect("/admin/dashboard");
                }

                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                _logger.LogInformation("User '{UserName}' requires two-factor authentication", user.UserName);
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User '{UserName}' account locked out", user.UserName);
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            _logger.LogWarning("Login failed: Invalid password for user '{UserId}'", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for '{UserNameOrEmail}'", Input.UserNameOrEmail);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
        }

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        return Page();
    }

    private async Task<ApplicationUser?> FindUserAsync(string userNameOrEmail)
    {
        // Try to find by email first
        var user = await _userManager.FindByEmailAsync(userNameOrEmail);

        // If not found by email, try username
        if (user == null)
        {
            user = await _userManager.FindByNameAsync(userNameOrEmail);
        }

        return user;
    }
}
