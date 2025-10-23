using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer.Domain.Entities;

namespace IdentityServer.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegisterModel> _logger;
    // TODO: Add IEmailSender when implementing email confirmation

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

    public class InputModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "You must agree to the Terms of Service")]
        [Display(Name = "I agree to the Terms of Service and Privacy Policy")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the Terms of Service")]
        public bool AgreeToTerms { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            Response.Redirect(returnUrl ?? "/");
            return;
        }

        ReturnUrl = returnUrl;
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
            // Check if email already exists
            var existingEmail = await _userManager.FindByEmailAsync(Input.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                _logger.LogWarning("Registration failed: Email '{Email}' already exists", Input.Email);
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            // Check if username already exists
            var existingUsername = await _userManager.FindByNameAsync(Input.UserName);
            if (existingUsername != null)
            {
                ModelState.AddModelError(string.Empty, "Username is already taken.");
                _logger.LogWarning("Registration failed: Username '{UserName}' already exists", Input.UserName);
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            // Create new user
            // Parse full name into first and last name
            var nameParts = Input.FullName.Trim().Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            var user = new ApplicationUser
            {
                UserName = Input.UserName,
                Email = Input.Email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = false, // Will be confirmed via email
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User '{UserName}' created successfully", user.UserName);

                // TODO: Send email confirmation
                // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { userId = user.Id, code = code }, protocol: Request.Scheme);
                // await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                // For now, auto-confirm email (remove this in production)
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);
                _logger.LogWarning("Auto-confirmed email for user '{UserName}' - remove this in production!", user.UserName);

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User '{UserName}' signed in after registration", user.UserName);

                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogWarning("Registration failed for '{UserName}': {ErrorCode} - {ErrorDescription}",
                    Input.UserName, error.Code, error.Description);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during registration for '{UserName}'", Input.UserName);
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
        }

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        return Page();
    }
}
