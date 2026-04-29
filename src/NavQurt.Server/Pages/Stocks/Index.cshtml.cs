using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Stocks;

public class IndexModel(IStockService stockService, IWarehouseService warehouseService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? WarehouseId { get; set; }

    public IReadOnlyCollection<IngredientStockDto> Stocks { get; private set; } = Array.Empty<IngredientStockDto>();
    public IReadOnlyCollection<WarehouseDto> Warehouses { get; private set; } = Array.Empty<WarehouseDto>();

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Warehouses = (await warehouseService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>();
        Stocks = (await stockService.GetBalancesAsync(WarehouseId, cancellationToken)).Value ?? Array.Empty<IngredientStockDto>();
    }
}
