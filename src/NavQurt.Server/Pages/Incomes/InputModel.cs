namespace NavQurt.Server.Pages.Incomes;

public class IncomeInput
{
    public int WarehouseId { get; set; }
    public DateTime IncomedAt { get; set; } = DateTime.Now;
    public string? Comment { get; set; }
    public List<IncomeItemInput> Items { get; set; } = [];
}

public class IncomeItemInput
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total => Quantity * Price;
}
