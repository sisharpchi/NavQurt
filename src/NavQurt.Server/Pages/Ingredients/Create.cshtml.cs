using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Ingredients;

public class CreateModel(IIngredientService ingredientService, IIngredientCategoryService categoryService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new() { IsActive = true, Unit = "pcs" };
    public IReadOnlyCollection<IngredientCategoryDto> Categories { get; private set; } = Array.Empty<IngredientCategoryDto>();
    [TempData] public string? Message { get; set; }
    public async Task OnGet(CancellationToken cancellationToken) => await LoadAsync(cancellationToken);
    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var result = await ingredientService.CreateAsync(new IngredientRequest(Input.Title, Input.Unit, Input.MinRemainderLimit, Input.AverageSelfPrice, Input.IsActive, Input.IngredientCategoryId), cancellationToken);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error); await LoadAsync(cancellationToken); return Page(); }
        Message = "Ingredient yaratildi.";
        return RedirectToPage("./Index");
    }
    private async Task LoadAsync(CancellationToken cancellationToken) => Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientCategoryDto>();
    public class InputModel { public string Title { get; set; } = string.Empty; public string Unit { get; set; } = "pcs"; public decimal MinRemainderLimit { get; set; } public decimal AverageSelfPrice { get; set; } public bool IsActive { get; set; } public int? IngredientCategoryId { get; set; } }
}