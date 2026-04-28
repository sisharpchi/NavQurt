namespace NavQurt.Core.Entities.Business;

public class PaymentMethod : IEntity<int>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
}
