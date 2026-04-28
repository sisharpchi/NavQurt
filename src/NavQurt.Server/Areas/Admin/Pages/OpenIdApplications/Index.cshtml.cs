using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NavQurt.Core.Entities;
using NavQurt.Infrastructure.Data;

namespace NavQurt.Server.Areas.Admin.Pages.OpenIdApplications;

public class IndexModel : PageModel
{
    private readonly MainDbContext _dbContext;

    public IndexModel(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<OpenIdApplication> Applications { get; set; } = [];

    public async Task OnGetAsync()
    {
        Applications = await _dbContext.Set<OpenIdApplication>()
            .AsNoTracking()
            .OrderBy(x => x.ClientId)
            .ToListAsync();
    }
}
