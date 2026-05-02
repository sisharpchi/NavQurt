using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Recipes;

public class DeleteModel(
    IRecipeService recipeService,
    IProductService productService,
    IIngredientService ingredientService) : PageModel
{
    public RecipeDto? Recipe { get; private set; }
    public string TargetTitle { get; private set; } = string.Empty;

    [TempData]
    public string? Message { get; set; }

    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        var result = await recipeService.GetAsync(id, cancellationToken);
        if (!result.Success || result.Value == null)
        {
            Message = result.Error ?? "Recipe topilmadi.";
            return RedirectToPage("./Index");
        }

        Recipe = result.Value;
        TargetTitle = await GetTargetTitleAsync(result.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPost(int id, CancellationToken cancellationToken)
    {
        var result = await recipeService.DeleteAsync(id, cancellationToken);
        Message = result.Success ? "Recipe o'chirildi." : result.Error;
        return RedirectToPage("./Index");
    }

    private async Task<string> GetTargetTitleAsync(RecipeDto recipe, CancellationToken cancellationToken)
    {
        if (recipe.ProductId.HasValue)
        {
            return (await productService.GetAsync(recipe.ProductId.Value, cancellationToken)).Value?.Title ?? "-";
        }

        return recipe.IngredientId.HasValue
            ? (await ingredientService.GetAsync(recipe.IngredientId.Value, cancellationToken)).Value?.Title ?? "-"
            : "-";
    }
}
