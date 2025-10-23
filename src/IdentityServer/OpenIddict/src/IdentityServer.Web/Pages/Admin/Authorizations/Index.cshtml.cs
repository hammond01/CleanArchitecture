using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Web.Pages.Admin.Authorizations;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
