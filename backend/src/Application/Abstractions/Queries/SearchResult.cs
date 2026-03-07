namespace Application.Abstractions.Queries;

public sealed class SearchResult<T>
{
    public SearchResult() { }

    public SearchResult(long total, IEnumerable<T> objects, int requestedPage, int? requestedObjectsCount)
    {
        Total = total;
        Objects = [.. objects];
        RequestedPage = requestedPage;
        RequestedObjectsCount = requestedObjectsCount;
    }

    public long Total { get; set; }
    public IReadOnlyList<T> Objects { get; set; } = [];
    public int RequestedPage { get; set; }
    public int? RequestedObjectsCount { get; set; }
}

public static class SearchResultExtensions
{
    public static SearchResult<TOut> Map<TIn, TOut>(this SearchResult<TIn> source, Func<TIn, TOut> selector)
    {
        return new SearchResult<TOut>(
            total: source.Total,
            objects: source.Objects.Select(selector),
            requestedPage: source.RequestedPage,
            requestedObjectsCount: source.RequestedObjectsCount
        );
    }
}
