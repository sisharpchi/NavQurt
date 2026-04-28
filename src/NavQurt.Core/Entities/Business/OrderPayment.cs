namespace NavQurt.Core.Entities.Business;

public class OrderPayment : IEntity<int>
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }

    public Order Order { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
}
