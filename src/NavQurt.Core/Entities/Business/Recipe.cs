namespace NavQurt.Core.Entities.Business;

public class Recipe : IEntity<int>
{
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public int? IngredientId { get; set; }
    public decimal PortionYield { get; set; } = 1;
    public bool IsDeleted { get; set; }

    public Product? Product { get; set; }
    public Ingredient? Ingredient { get; set; }
    public ICollection<RecipeItem> Items { get; set; } = new List<RecipeItem>();
}
