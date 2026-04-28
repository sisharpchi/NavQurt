namespace NavQurt.Core.Entities.Business;

public class Warehouse : IEntity<int>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }

    public ICollection<IngredientStock> IngredientStocks { get; set; } = new List<IngredientStock>();
}
