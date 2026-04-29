using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Incomes;

public class DetailModel(IIncomeService incomeService, IWarehouseService warehouseService) : PageModel
{
    public IncomeDto? Income { get; private set; }
    public IReadOnlyCollection<WarehouseDto> Warehouses { get; private set; } = Array.Empty<WarehouseDto>();
    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        Warehouses = (await warehouseService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>();
        var result = await incomeService.GetAsync(id, cancellationToken);
        if (!result.Success) return NotFound();
        Income = result.Value;
        return Page();
    }
    public string GetWarehouseTitle(int id) => Warehouses.FirstOrDefault(x => x.Id == id)?.Title ?? "-";
}