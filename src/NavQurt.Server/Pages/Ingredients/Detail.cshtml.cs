using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Ingredients;

public class DetailModel(IIngredientService ingredientService, IIngredientCategoryService categoryService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public IReadOnlyCollection<IngredientCategoryDto> Categories { get; private set; } = Array.Empty<IngredientCategoryDto>();
    [TempData] public string? Message { get; set; }
    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        var result = await ingredientService.GetAsync(id, cancellationToken);
        if (!result.Success) return NotFound();
        Input = new InputModel { Title = result.Value.Title, Unit = result.Value.Unit, MinRemainderLimit = result.Value.MinRemainderLimit, AverageSelfPrice = result.Value.AverageSelfPrice, IsActive = result.Value.IsActive, IngredientCategoryId = result.Value.IngredientCategoryId };
        return Page();
    }
    public async Task<IActionResult> OnPost(int id, CancellationToken cancellationToken)
    {
        var result = await ingredientService.UpdateAsync(id, new IngredientRequest(Input.Title, Input.Unit, Input.MinRemainderLimit, Input.AverageSelfPrice, Input.IsActive, Input.IngredientCategoryId), cancellationToken);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error); await LoadAsync(cancellationToken); return Page(); }
        Message = "Ingredient saqlandi.";
        return RedirectToPage("./Index");
    }
    private async Task LoadAsync(CancellationToken cancellationToken) => Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientCategoryDto>();
    public class InputModel { public string Title { get; set; } = string.Empty; public string Unit { get; set; } = string.Empty; public decimal MinRemainderLimit { get; set; } public decimal AverageSelfPrice { get; set; } public bool IsActive { get; set; } public int? IngredientCategoryId { get; set; } }
}