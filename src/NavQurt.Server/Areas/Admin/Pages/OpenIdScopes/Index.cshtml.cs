using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NavQurt.Core.Entities;
using NavQurt.Infrastructure.Data;

namespace NavQurt.Server.Areas.Admin.Pages.OpenIdScopes;

public class IndexModel : PageModel
{
    private readonly MainDbContext _dbContext;

    public IndexModel(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<OpenIdScope> Scopes { get; set; } = [];

    public async Task OnGetAsync()
    {
        Scopes = await _dbContext.Set<OpenIdScope>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}
