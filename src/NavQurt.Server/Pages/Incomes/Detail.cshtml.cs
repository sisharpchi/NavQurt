using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Incomes;

public class DetailModel(IIncomeService incomeService) : PageModel
{
    public IncomeDto? Income { get; private set; }

    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        var result = await incomeService.GetAsync(id, cancellationToken);
        if (!result.Success) return NotFound();
        Income = result.Value;
        return Page();
    }
}
