using System.Text.Json.Serialization;

namespace NavQurt.Shared;

public record PagedResponseResult<T> : IPagedResponseResult<T>, IResponseResult
{
    public PagedResponseResult()
        : this(null, 0, 0)
    {
    }

    public PagedResponseResult(IEnumerable<T>? items, int itemsCount, int pagesCount)
    {
        Items = items ?? [];
        ItemsCount = itemsCount;
        PagesCount = pagesCount;
    }

    [JsonPropertyName("items")]
    public IEnumerable<T> Items { get; init; }

    [JsonPropertyName("items_count")]
    public int ItemsCount { get; init; }

    [JsonPropertyName("pages_count")]
    public int PagesCount { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Error { get; init; }

    public bool Success => string.IsNullOrEmpty(Error);

    public static PagedResponseResult<T> CreateError(string error) => new(null, 0, 0) { Error = error };
}

public record MovementPagination<T> : PagedResponseResult<T>
{
    public MovementPagination(IEnumerable<T>? items, int itemsCount, int pagesCount, decimal total)
        : base(items, itemsCount, pagesCount)
    {
        Total = total;
    }

    public decimal Total { get; set; }
}

public record IngredientWriteoffPagination<T> : MovementPagination<T>
{
    public IngredientWriteoffPagination(IEnumerable<T>? items, int itemsCount, int pagesCount, decimal total)
        : base(items, itemsCount, pagesCount, total)
    {
    }
}

public record PaginationCanceledObject<TModel> : PagedResponseResult<TModel>
{
    public decimal CanceledAmount { get; set; }
    public decimal ProductsCanceledQuantity { get; set; }
}

public record PaginationPerDaysObject<TModel> : PagedResponseResult<TModel>
{
    public decimal Total { get; set; }
    public decimal TotalCash { get; set; }
    public decimal TotalCard { get; set; }
    public int TotalOrdersCount { get; set; }
    public decimal TotalExternalAmount { get; set; }
    public decimal TotalDebt { get; set; }
    public decimal TotalPayedDebt { get; set; }
}
