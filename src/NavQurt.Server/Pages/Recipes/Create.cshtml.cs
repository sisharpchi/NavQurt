using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Recipes;

public class CreateModel(
    IRecipeService recipeService,
    IProductService productService,
    IIngredientService ingredientService) : PageModel
{
    private const int RecipeRows = 10;

    [BindProperty]
    public RecipeInput Input { get; set; } = RecipeInput.CreateEmpty(RecipeRows);

    [BindProperty(SupportsGet = true)]
    public string? TargetType { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? ProductId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? IngredientId { get; set; }

    public IReadOnlyCollection<ProductDto> Products { get; private set; } = Array.Empty<ProductDto>();
    public IReadOnlyCollection<IngredientDto> Ingredients { get; private set; } = Array.Empty<IngredientDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Input = RecipeInput.CreateEmpty(RecipeRows);
        if (string.Equals(TargetType, "Ingredient", StringComparison.OrdinalIgnoreCase))
        {
            Input.TargetType = "Ingredient";
        }

        Input.ProductId = ProductId;
        Input.IngredientId = IngredientId;
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        var result = await recipeService.UpsertAsync(CreateRequest(), cancellationToken);
        if (result.Success)
        {
            Message = "Recipe saqlandi.";
            return RedirectToPage("./Detail", new { id = result.Value!.Id });
        }

        Message = result.Error;
        return Page();
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

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Products = (await productService.GetListAsync(new ProductListRequest(IsActive: true), cancellationToken)).Value?.Items ?? Array.Empty<ProductDto>();
        Ingredients = (await ingredientService.GetListAsync(new IngredientListRequest(IsActive: true), cancellationToken)).Value?.Items ?? Array.Empty<IngredientDto>();
    }
}
