namespace NavQurt.Server.Pages.Recipes;

public class RecipeInput
{
    public string TargetType { get; set; } = "Product";
    public int? ProductId { get; set; }
    public int? IngredientId { get; set; }
    public decimal PortionYield { get; set; } = 1;
    public List<RecipeItemInput> Items { get; set; } = new();

    public static RecipeInput CreateEmpty(int rows = 10) => new()
    {
        TargetType = "Product",
        PortionYield = 1,
        Items = Enumerable.Range(0, rows).Select(_ => new RecipeItemInput { Quantity = 1 }).ToList()
    };
}

public class RecipeItemInput
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
}
