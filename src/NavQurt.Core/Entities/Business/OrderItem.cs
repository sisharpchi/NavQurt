namespace NavQurt.Core.Entities.Business;

public class OrderItem : IEntity<int>
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total => Quantity * Price;
    public bool IsCombo { get; set; }

    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
