namespace NavQurt.Shared;

public record OrderPaymentPagedResponseResult<T> : PagedResponseResult<T>
{
    public OrderPaymentPagedResponseResult()
    {
    }

    public OrderPaymentPagedResponseResult(
        IEnumerable<T>? items,
        int itemsCount,
        int pagesCount,
        decimal total,
        decimal totalCash,
        decimal totalCard,
        decimal totalService,
        Dictionary<string, decimal> payments)
        : base(items, itemsCount, pagesCount)
    {
        Total = total;
        TotalCash = totalCash;
        TotalCard = totalCard;
        TotalService = totalService;
        Payments = payments;
    }

    public decimal Total { get; set; }
    public decimal TotalCash { get; set; }
    public decimal TotalCard { get; set; }
    public decimal TotalService { get; set; }
    public Dictionary<string, decimal> Payments { get; } = [];
}

public record OrderTablePagedResponseResult<T> : PagedResponseResult<T>
{
    public OrderTablePagedResponseResult()
    {
    }

    public OrderTablePagedResponseResult(
        IEnumerable<T>? items,
        int itemsCount,
        int pagesCount,
        decimal total,
        decimal mostOverPricedTableTotal,
        string mostOverPricedTable,
        int mostOverPricedTableOrderId)
        : base(items, itemsCount, pagesCount)
    {
        Total = total;
        MostOverPricedTable = mostOverPricedTable;
        MostOverPricedTableTotal = mostOverPricedTableTotal;
        MostOverPricedTableOrderId = mostOverPricedTableOrderId;
    }

    public decimal Total { get; set; }
    public string MostOverPricedTable { get; set; } = string.Empty;
    public decimal MostOverPricedTableTotal { get; set; }
    public int MostOverPricedTableOrderId { get; set; }
}

public record TotalPagedResponseResult<T> : PagedResponseResult<T>
{
    public TotalPagedResponseResult()
    {
    }

    public TotalPagedResponseResult(IEnumerable<T>? items, int itemsCount, int pagesCount, decimal total)
        : base(items, itemsCount, pagesCount)
    {
        Total = total;
    }

    public decimal Total { get; set; }
}

public record TransactionPagedResposeResult<T> : PagedResponseResult<T>
{
    public TransactionPagedResposeResult()
    {
    }

    public TransactionPagedResposeResult(IEnumerable<T>? items, int itemsCount, int pagesCount, decimal totalIncome, decimal totalOutcome)
        : base(items, itemsCount, pagesCount)
    {
        TotalIncome = totalIncome;
        TotalOutcome = totalOutcome;
    }

    public decimal TotalIncome { get; set; }
    public decimal TotalOutcome { get; set; }
}
