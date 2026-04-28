namespace NavQurt.Core.Entities.Business;

public class Worker : IEntity<int>
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
