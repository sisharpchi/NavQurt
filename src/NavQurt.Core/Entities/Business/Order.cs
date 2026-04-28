using NavQurt.Core.Enumerations;

namespace NavQurt.Core.Entities.Business;

public class Order : IEntity<int>
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int WorkerId { get; set; }
    public int WarehouseId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Paid;
    public string CustomerPhoneNumber { get; set; } = string.Empty;
    public string CustomerFullName { get; set; } = string.Empty;
    public string? CustomerLocation { get; set; }
    public string? Comment { get; set; }

    public Customer Customer { get; set; } = null!;
    public Worker Worker { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<OrderPayment> Payments { get; set; } = new List<OrderPayment>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
