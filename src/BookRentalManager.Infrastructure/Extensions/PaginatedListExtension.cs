namespace BookRentalManager.Infrastructure.Extensions;

public static class PaginatedListExtension
{
    private const int MaxItemsPerPage = 50;

    public static async Task<IReadOnlyList<TItem>> ToReadOnlyPaginatedListAsync<TItem>(
        this IQueryable<TItem> items,
        CancellationToken cancellationToken,
        int pageIndex,
        int totalItemsPerPage
    )
    {
        if (!items.Any())
        {
            return new List<TItem>();
        }
        var actualTotalItemsPerPage = totalItemsPerPage > MaxItemsPerPage ? MaxItemsPerPage : totalItemsPerPage;
        var totalPages = (int)Math.Ceiling(items.Count() / (double)actualTotalItemsPerPage);
        var actualPageIndex = pageIndex > totalPages ? totalPages : pageIndex;
        var paginatedItems = await items
            .Skip((actualPageIndex - 1) * actualTotalItemsPerPage)
            .Take(actualTotalItemsPerPage)
            .ToListAsync(cancellationToken);
        return (paginatedItems).AsReadOnly();
    }
}
