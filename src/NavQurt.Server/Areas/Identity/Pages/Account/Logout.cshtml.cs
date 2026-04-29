using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NavQurt.Server.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LogoutModel : PageModel
{
    private readonly SignInManager<NavQurt.Core.Entities.User> _signInManager;

    public LogoutModel(SignInManager<NavQurt.Core.Entities.User> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await SignOutAsync();
        return LocalRedirect("~/login1");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await SignOutAsync();
        return LocalRedirect("~/login1");
    }

    private async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
    }
}
