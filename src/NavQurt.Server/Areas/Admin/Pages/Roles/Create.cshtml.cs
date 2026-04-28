using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Core.Entities;

namespace NavQurt.Server.Areas.Admin.Pages.Roles;

public class CreateModel : PageModel
{
    private readonly RoleManager<AppRole> _roleManager;

    public CreateModel(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var role = new AppRole(Input.Name)
        {
            DisplayName = string.IsNullOrWhiteSpace(Input.DisplayName) ? Input.Name : Input.DisplayName
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        return RedirectToPage("./Index");
    }

    public class InputModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Display name")]
        public string? DisplayName { get; set; }
    }
}
