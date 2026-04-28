namespace NavQurt.Shared;

public interface IResponseResult
{
    bool Success { get; }
    string? Error { get; init; }
}

public interface IPagedResponseResult<T>
{
    IEnumerable<T> Items { get; init; }
    int ItemsCount { get; init; }
    int PagesCount { get; init; }
}
