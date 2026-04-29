using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Core.Entities;
using NavQurt.Core.Enumerations;
using NavQurt.Shared.Claims;

namespace NavQurt.Server.Pages;

[AllowAnonymous]
public class Login2Model : PageModel
{
    private readonly SignInManager<User> _signInManager;

    public Login2Model(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public async Task OnGetAsync()
    {
        ReturnUrl ??= Url.Content("~/Admin/Users");
        await _signInManager.SignOutAsync();
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ReturnUrl ??= Url.Content("~/Admin/Users");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _signInManager.UserManager.FindByNameAsync(Input.UserName);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            Input.Password,
            lockoutOnFailure: false);

        if (result.Succeeded && await _signInManager.UserManager.IsInRoleAsync(user, AppRoles.SuperAdmin))
        {
            await _signInManager.SignInWithClaimsAsync(
                user,
                Input.RememberMe,
                [new Claim(CustomClaimTypes.LoginMode, CustomClaimValues.AdminLogin)]);

            return LocalRedirect(ReturnUrl);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }

    public class InputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
