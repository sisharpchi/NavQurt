using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NavQurt.Core.Entities;

namespace NavQurt.Server.Areas.Admin.Pages.Users;

public class IndexModel : PageModel
{
    private readonly UserManager<User> _userManager;

    public IndexModel(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public List<UserDisplay> Users { get; set; } = [];

    public async Task OnGetAsync()
    {
        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Search))
        {
            query = query.Where(x =>
                x.UserName!.Contains(Search) ||
                x.PhoneNumber!.Contains(Search));
        }

        var users = await query
            .OrderBy(x => x.UserName)
            .Take(100)
            .ToListAsync();

        foreach (var user in users)
        {
            Users.Add(new UserDisplay(
                user.Id,
                user.UserName,
                user.PhoneNumber,
                user.FullName,
                await _userManager.GetRolesAsync(user)));
        }
    }
}

public record UserDisplay(
    string Id,
    string? UserName,
    string? PhoneNumber,
    string FullName,
    IEnumerable<string> Roles);
