namespace NavQurt.Core.Entities.Business;

public class RecipeItem : IEntity<int>
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }

    public Recipe Recipe { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}
