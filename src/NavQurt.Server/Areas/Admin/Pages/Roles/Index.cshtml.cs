using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NavQurt.Core.Entities;

namespace NavQurt.Server.Areas.Admin.Pages.Roles;

public class IndexModel : PageModel
{
    private readonly RoleManager<AppRole> _roleManager;

    public IndexModel(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public List<AppRole> Roles { get; set; } = [];

    public async Task OnGetAsync()
    {
        Roles = await _roleManager.Roles
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}
