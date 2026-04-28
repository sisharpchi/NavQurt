namespace NavQurt.Core.Entities.Business;

public class Customer : IEntity<int>
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
