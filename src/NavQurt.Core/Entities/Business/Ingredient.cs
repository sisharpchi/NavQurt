namespace NavQurt.Core.Entities.Business;

public class Ingredient : IEntity<int>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Unit { get; set; } = "pcs";
    public decimal MinRemainderLimit { get; set; }
    public decimal AverageSelfPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public int? IngredientCategoryId { get; set; }

    public IngredientCategory? IngredientCategory { get; set; }
    public Recipe? Recipe { get; set; }
    public ICollection<RecipeItem> RecipeItems { get; set; } = new List<RecipeItem>();
    public ICollection<IngredientStock> Stocks { get; set; } = new List<IngredientStock>();
}
