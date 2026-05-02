using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Stocks;

public class IndexModel(
    IStockService stockService,
    IWarehouseService warehouseService,
    IIngredientCategoryService categoryService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? WarehouseId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? IngredientTitle { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool OnlyOutOfStock { get; set; }

    public IReadOnlyCollection<IngredientStockDto> Stocks { get; private set; } = Array.Empty<IngredientStockDto>();
    public IReadOnlyCollection<WarehouseDto> Warehouses { get; private set; } = Array.Empty<WarehouseDto>();
    public IReadOnlyCollection<IngredientCategoryDto> Categories { get; private set; } = Array.Empty<IngredientCategoryDto>();
    public decimal Total { get; private set; }
    public decimal TotalWithoutNegativeRemainder { get; private set; }
    public int ItemsCount { get; private set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Warehouses = (await warehouseService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>();
        Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientCategoryDto>();

        var request = new StockBalanceRequest(WarehouseId, CategoryId, IngredientTitle, OnlyOutOfStock);
        var result = (await stockService.GetBalancesAsync(request, cancellationToken)).Value;

        Stocks = result?.Items ?? Array.Empty<IngredientStockDto>();
        Total = result?.Total ?? 0;
        TotalWithoutNegativeRemainder = result?.TotalWithoutNegativeRemainder ?? 0;
        ItemsCount = result?.ItemsCount ?? 0;
    }
}
