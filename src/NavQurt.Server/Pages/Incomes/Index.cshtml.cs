using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Incomes;

public class IndexModel(IIncomeService incomeService, IWarehouseService warehouseService) : PageModel
{
    public IReadOnlyCollection<IncomeDto> Incomes { get; private set; } = Array.Empty<IncomeDto>();
    public IReadOnlyCollection<WarehouseDto> Warehouses { get; private set; } = Array.Empty<WarehouseDto>();
    [TempData] public string? Message { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime FromDate { get; set; } = DateTime.Today.AddDays(-7);
    [BindProperty(SupportsGet = true)] public DateTime ToDate { get; set; } = DateTime.Today;
    [BindProperty(SupportsGet = true)] public int? WarehouseId { get; set; }
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    public decimal Total { get; private set; }
    public int ItemsCount { get; private set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Warehouses = (await warehouseService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>();
        var request = new IncomeListRequest(FromDate, ToDate, WarehouseId, Search);
        var result = (await incomeService.GetListAsync(request, cancellationToken)).Value;

        Incomes = result?.Items ?? Array.Empty<IncomeDto>();
        Total = result?.Total ?? 0;
        ItemsCount = result?.ItemsCount ?? 0;
    }
}
