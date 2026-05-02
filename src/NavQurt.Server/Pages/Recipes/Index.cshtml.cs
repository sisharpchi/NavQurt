using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Recipes;

public class IndexModel(IRecipeService recipeService) : PageModel
{
    public IReadOnlyCollection<RecipeListItemDto> Recipes { get; private set; } = Array.Empty<RecipeListItemDto>();
    public int ItemsCount { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? TargetType { get; set; }

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var result = await recipeService.GetListAsync(new RecipeListRequest(Search, TargetType), cancellationToken);
        Recipes = result.Value?.Items ?? Array.Empty<RecipeListItemDto>();
        ItemsCount = result.Value?.ItemsCount ?? Recipes.Count;
    }
}
