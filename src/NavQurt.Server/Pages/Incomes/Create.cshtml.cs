using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Incomes;

public class CreateModel(IIncomeService incomeService, IWarehouseService warehouseService, IIngredientService ingredientService) : PageModel
{
    private const int IncomeRows = 10;
    [BindProperty] public IncomeInput Input { get; set; } = CreateEmptyInput();
    public IReadOnlyCollection<WarehouseDto> Warehouses { get; private set; } = Array.Empty<WarehouseDto>();
    public IReadOnlyCollection<IngredientDto> Ingredients { get; private set; } = Array.Empty<IngredientDto>();
    [TempData] public string? Message { get; set; }
    public async Task OnGet(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        Input.WarehouseId = Warehouses.FirstOrDefault(x => x.IsMain)?.Id ?? Warehouses.FirstOrDefault()?.Id ?? 0;
    }
    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var items = Input.Items.Where(x => x.IngredientId > 0 && x.Quantity > 0).Select(x => new CreateIncomeItemRequest(x.IngredientId, x.Quantity, x.Price)).ToList();
        var result = await incomeService.CreateAsync(new CreateIncomeRequest(Input.WarehouseId, Input.Comment, items), cancellationToken);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error); await LoadAsync(cancellationToken); return Page(); }
        Message = "Prixod qilindi.";
        return RedirectToPage("./Index");
    }
    private async Task LoadAsync(CancellationToken cancellationToken) { Warehouses = (await warehouseService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>(); Ingredients = (await ingredientService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientDto>(); }
    private static IncomeInput CreateEmptyInput() => new() { Items = Enumerable.Range(0, IncomeRows).Select(_ => new IncomeItemInput()).ToList() };
    public class IncomeInput { public int WarehouseId { get; set; } public string? Comment { get; set; } public List<IncomeItemInput> Items { get; set; } = new(); }
    public class IncomeItemInput { public int IngredientId { get; set; } public decimal Quantity { get; set; } public decimal Price { get; set; } }
}