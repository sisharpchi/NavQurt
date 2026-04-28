using NavQurt.Core.Enumerations;

namespace NavQurt.Core.Entities.Business;

public class StockMovement : IEntity<int>
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public decimal BalanceAfter { get; set; }
    public StockMovementType Type { get; set; }
    public int? IncomeId { get; set; }
    public int? OrderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Comment { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
    public Income? Income { get; set; }
    public Order? Order { get; set; }
}
