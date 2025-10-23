using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Web.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(
        SignInManager<ApplicationUser> signInManager,
        ILogger<LogoutModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        var userName = User.Identity?.Name;

        // Sign out from ASP.NET Core Identity
        await _signInManager.SignOutAsync();

        _logger.LogInformation("User '{UserName}' logged out successfully", userName);

        // Redirect to login or return URL
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToPage("/Account/Login");
    }

    public IActionResult OnGet()
    {
        // Logout should only be done via POST for security
        return RedirectToPage("/Account/Login");
    }
}
