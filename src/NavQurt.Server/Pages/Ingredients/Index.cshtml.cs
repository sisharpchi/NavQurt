using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Ingredients;

public class IndexModel(IIngredientService ingredientService, IIngredientCategoryService categoryService) : PageModel
{
    public IReadOnlyCollection<IngredientDto> Ingredients { get; private set; } = Array.Empty<IngredientDto>();
    public IReadOnlyCollection<IngredientCategoryDto> Categories { get; private set; } = Array.Empty<IngredientCategoryDto>();
    public int ItemsCount { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool OnlyWithoutRecipe { get; set; }

    [TempData] public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var result = await ingredientService.GetListAsync(
            new IngredientListRequest(Search, CategoryId, ParseBool(Status), OnlyWithoutRecipe),
            cancellationToken);

        Ingredients = result.Value?.Items ?? Array.Empty<IngredientDto>();
        ItemsCount = result.Value?.ItemsCount ?? Ingredients.Count;
        Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientCategoryDto>();
    }

    public string GetCategoryTitle(IngredientDto item) =>
        item.IngredientCategoryTitle ?? (item.IngredientCategoryId.HasValue ? Categories.FirstOrDefault(x => x.Id == item.IngredientCategoryId.Value)?.Title ?? "-" : "-");

    private static bool? ParseBool(string? value) =>
        bool.TryParse(value, out var parsed) ? parsed : null;
}
