using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Incomes;

public class IndexModel(IIncomeService incomeService, IWarehouseService warehouseService) : PageModel
{
    public IReadOnlyCollection<IncomeDto> Incomes { get; private set; } = Array.Empty<IncomeDto>();
    public IReadOnlyCollection<WarehouseDto> Warehouses { get; private set; } = Array.Empty<WarehouseDto>();
    [TempData] public string? Message { get; set; }
    public async Task OnGet(CancellationToken cancellationToken)
    {
        Incomes = (await incomeService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IncomeDto>();
        Warehouses = (await warehouseService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>();
    }
    public string GetWarehouseTitle(int id) => Warehouses.FirstOrDefault(x => x.Id == id)?.Title ?? "-";
}
