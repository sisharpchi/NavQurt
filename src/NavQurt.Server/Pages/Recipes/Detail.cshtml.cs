using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Recipes;

public class DetailModel(
    IRecipeService recipeService,
    IProductService productService,
    IIngredientService ingredientService) : PageModel
{
    private const int RecipeRows = 10;

    [BindProperty]
    public RecipeInput Input { get; set; } = RecipeInput.CreateEmpty(RecipeRows);

    public int Id { get; private set; }
    public string TargetTitle { get; private set; } = string.Empty;
    public IReadOnlyCollection<ProductDto> Products { get; private set; } = Array.Empty<ProductDto>();
    public IReadOnlyCollection<IngredientDto> Ingredients { get; private set; } = Array.Empty<IngredientDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        Id = id;
        await LoadLookupsAsync(cancellationToken);

        var result = await recipeService.GetAsync(id, cancellationToken);
        if (!result.Success || result.Value == null)
        {
            Message = result.Error ?? "Recipe topilmadi.";
            return RedirectToPage("./Index");
        }

        PopulateInput(result.Value);
        return Page();
    }

    public async Task<IActionResult> OnPost(int id, CancellationToken cancellationToken)
    {
        Id = id;
        await LoadLookupsAsync(cancellationToken);

        var result = await recipeService.UpsertAsync(CreateRequest(), cancellationToken);
        if (result.Success)
        {
            Message = "Recipe yangilandi.";
            return RedirectToPage("./Detail", new { id = result.Value!.Id });
        }

        Message = result.Error;
        TargetTitle = GetTargetTitle(Input.TargetType, Input.ProductId, Input.IngredientId);
        EnsureRows();
        return Page();
    }

    private void PopulateInput(RecipeDto recipe)
    {
        Input = RecipeInput.CreateEmpty(RecipeRows);
        Input.TargetType = recipe.ProductId.HasValue ? "Product" : "Ingredient";
        Input.ProductId = recipe.ProductId;
        Input.IngredientId = recipe.IngredientId;
        Input.PortionYield = recipe.PortionYield;
        Input.Items = recipe.Items
            .Select(x => new RecipeItemInput { IngredientId = x.IngredientId, Quantity = x.Quantity })
            .ToList();
        EnsureRows();
        TargetTitle = GetTargetTitle(Input.TargetType, Input.ProductId, Input.IngredientId);
    }

    private void EnsureRows()
    {
        while (Input.Items.Count < RecipeRows)
        {
            Input.Items.Add(new RecipeItemInput { Quantity = 1 });
        }
    }

    private UpsertRecipeRequest CreateRequest()
    {
        var productId = Input.TargetType == "Product" ? Input.ProductId : null;
        var ingredientId = Input.TargetType == "Ingredient" ? Input.IngredientId : null;
        var items = Input.Items
            .Where(x => x.IngredientId > 0 && x.Quantity > 0)
            .Select(x => new RecipeItemRequest(x.IngredientId, x.Quantity))
            .ToList();

        return new UpsertRecipeRequest(productId, ingredientId, Input.PortionYield, items);
    }

    private async Task LoadLookupsAsync(CancellationToken cancellationToken)
    {
        Products = (await productService.GetListAsync(new ProductListRequest(IsActive: true), cancellationToken)).Value?.Items ?? Array.Empty<ProductDto>();
        Ingredients = (await ingredientService.GetListAsync(new IngredientListRequest(IsActive: true), cancellationToken)).Value?.Items ?? Array.Empty<IngredientDto>();
    }

    private string GetTargetTitle(string targetType, int? productId, int? ingredientId) =>
        targetType == "Product"
            ? Products.FirstOrDefault(x => x.Id == productId)?.Title ?? "-"
            : Ingredients.FirstOrDefault(x => x.Id == ingredientId)?.Title ?? "-";
}
