namespace BookRentalManager.Infrastructure.Extensions;

public static class PaginatedListExtension
{
    private const int MaxItemsPerPage = 50;

    public static async Task<IReadOnlyList<TItem>> ToReadOnlyPaginatedListAsync<TItem>(
        this IQueryable<TItem> items,
        CancellationToken cancellationToken,
        int pageIndex,
        int totalItemsPerPage)
    {
        if (!items.Any())
        {
            return new List<TItem>();
        }
        totalItemsPerPage = totalItemsPerPage > MaxItemsPerPage ? MaxItemsPerPage : totalItemsPerPage;
        var totalPages = (int)Math.Ceiling(items.Count() / (double)totalItemsPerPage);
        pageIndex = pageIndex > totalPages ? totalPages : pageIndex;
        var paginatedItems = await items
            .Skip((pageIndex - 1) * totalItemsPerPage)
            .Take(totalItemsPerPage)
            .ToListAsync(cancellationToken);
        return (paginatedItems).AsReadOnly();
    }
}
