using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Recipes;

public class IndexModel(IRecipeService recipeService, IProductService productService, IIngredientService ingredientService) : PageModel
{
    private const int RecipeRows = 10;

    [BindProperty]
    public RecipeInput Input { get; set; } = CreateEmptyInput();

    public IReadOnlyCollection<ProductDto> Products { get; private set; } = Array.Empty<ProductDto>();
    public IReadOnlyCollection<IngredientDto> Ingredients { get; private set; } = Array.Empty<IngredientDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var productId = Input.TargetType == "Product" ? Input.ProductId : null;
        var ingredientId = Input.TargetType == "Ingredient" ? Input.IngredientId : null;
        var items = Input.Items
            .Where(x => x.IngredientId > 0 && x.Quantity > 0)
            .Select(x => new RecipeItemRequest(x.IngredientId, x.Quantity))
            .ToList();

        var result = await recipeService.UpsertAsync(new UpsertRecipeRequest(productId, ingredientId, Input.PortionYield, items), cancellationToken);
        Message = result.Success ? "Recipe saqlandi." : result.Error;
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Products = (await productService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductDto>();
        Ingredients = (await ingredientService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientDto>();
    }

    private static RecipeInput CreateEmptyInput() => new()
    {
        TargetType = "Product",
        PortionYield = 1,
        Items = Enumerable.Range(0, RecipeRows).Select(_ => new RecipeItemInput { Quantity = 1 }).ToList()
    };

    public class RecipeInput
    {
        public string TargetType { get; set; } = "Product";
        public int? ProductId { get; set; }
        public int? IngredientId { get; set; }
        public decimal PortionYield { get; set; }
        public List<RecipeItemInput> Items { get; set; } = new();
    }

    public class RecipeItemInput
    {
        public int IngredientId { get; set; }
        public decimal Quantity { get; set; }
    }
}
