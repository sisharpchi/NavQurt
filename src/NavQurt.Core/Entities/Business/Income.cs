namespace NavQurt.Core.Entities.Business;

public class Income : IEntity<int>
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string? Comment { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<IncomeItem> Items { get; set; } = new List<IncomeItem>();
}
