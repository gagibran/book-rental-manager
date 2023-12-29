using BookRentalManager.Domain.Extensions;

namespace BookRentalManager.Domain.Common;

public sealed class PaginatedList<TItem> : List<TItem>
{
    private const int MaxItemsPerPage = 50;

    public int TotalAmountOfPages { get; }
    public int TotalAmountOfItems { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }

    public PaginatedList(
        List<TItem> items,
        int totalAmountOfItems,
        int totalAMountOfPages,
        int pageIndex,
        int pageSize)
    {
        TotalAmountOfItems = totalAmountOfItems;
        TotalAmountOfPages = totalAMountOfPages;
        PageIndex = pageIndex;
        PageSize = pageSize;
        HasNextPage = pageIndex < totalAMountOfPages;
        HasPreviousPage = pageIndex > 1;
        AddRange(items);
    }

    public static async Task<PaginatedList<TItem>> CreateAsync(
        IQueryable<TItem> items,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var paginatedItems = new List<TItem>();
        int totalAmountOfItems = items.Count();
        int totalAmountOfPages = (int)Math.Ceiling(totalAmountOfItems / (double)pageSize);
        int actualPageSize = pageSize > MaxItemsPerPage ? MaxItemsPerPage : pageSize;
        int actualPageIndex = pageIndex > totalAmountOfPages ? totalAmountOfPages : pageIndex;
        if (totalAmountOfItems > 0)
        {
            paginatedItems = await items
                .Skip((actualPageIndex - 1) * actualPageSize)
                .Take(actualPageSize)
                .ToListAsync(cancellationToken);
        }
        return new PaginatedList<TItem>(paginatedItems, totalAmountOfItems, totalAmountOfPages, actualPageIndex, actualPageSize);
    }
}
