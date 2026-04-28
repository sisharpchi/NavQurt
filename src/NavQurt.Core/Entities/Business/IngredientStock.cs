namespace NavQurt.Core.Entities.Business;

public class IngredientStock : IEntity<int>
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Warehouse Warehouse { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}
