using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Ingredients;

public class IndexModel(IIngredientService ingredientService, IIngredientCategoryService categoryService) : PageModel
{
    public IReadOnlyCollection<IngredientDto> Ingredients { get; private set; } = Array.Empty<IngredientDto>();
    public IReadOnlyCollection<IngredientCategoryDto> Categories { get; private set; } = Array.Empty<IngredientCategoryDto>();
    [TempData] public string? Message { get; set; }
    public async Task OnGet(CancellationToken cancellationToken)
    {
        Ingredients = (await ingredientService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientDto>();
        Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientCategoryDto>();
    }
    public string GetCategoryTitle(int? id) => id.HasValue ? Categories.FirstOrDefault(x => x.Id == id.Value)?.Title ?? "-" : "-";
}
