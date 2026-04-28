namespace NavQurt.Core.Entities.Business;

public class ProductComboItem : IEntity<int>
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int ComboProductId { get; set; }
    public decimal Quantity { get; set; }

    public Product Product { get; set; } = null!;
    public Product ComboProduct { get; set; } = null!;
}
