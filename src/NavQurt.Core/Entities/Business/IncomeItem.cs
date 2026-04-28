namespace NavQurt.Core.Entities.Business;

public class IncomeItem : IEntity<int>
{
    public int Id { get; set; }
    public int IncomeId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total => Quantity * Price;

    public Income Income { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}
