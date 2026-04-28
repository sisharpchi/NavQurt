using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NavQurt.Core.Entities;

namespace NavQurt.Server.Areas.Admin.Pages.Users;

public class CreateModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public CreateModel(UserManager<User> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList RoleOptions { get; set; } = default!;

    public async Task OnGetAsync()
    {
        await LoadRolesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadRolesAsync();
            return Page();
        }

        var user = new User
        {
            UserName = Input.UserName,
            PhoneNumber = Input.PhoneNumber,
            FirstName = Input.FirstName,
            LastName = Input.LastName
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await LoadRolesAsync();
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(Input.Role))
        {
            await _userManager.AddToRoleAsync(user, Input.Role);
        }

        return RedirectToPage("./Index");
    }

    private async Task LoadRolesAsync()
    {
        var roles = await _roleManager.Roles
            .OrderBy(x => x.Name)
            .Select(x => x.Name!)
            .ToListAsync();

        RoleOptions = new SelectList(roles);
    }

    public class InputModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "First name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last name")]
        public string? LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? Role { get; set; }
    }
}
