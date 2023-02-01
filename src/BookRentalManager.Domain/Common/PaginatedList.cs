using BookRentalManager.Domain.Extensions;

namespace BookRentalManager.Domain.Common;

public class PaginatedList<TItem> : List<TItem>
{
    private const int MaxItemsPerPage = 50;

    public int TotalAmountOfPages { get; }
    public int TotalAmountOfItems { get; }
    public int PageIndex { get; }
    public int TotalAmountOfItemsPerPage { get; }
    public bool HasNextPage => PageIndex < TotalAmountOfPages;
    public bool HasPreviousPage => PageIndex > 1;

    public PaginatedList(
        List<TItem> items,
        int pageIndex,
        int totalAmountOfItemsPerPage)
    {
        TotalAmountOfItems = items.Count();
        TotalAmountOfPages = (int)Math.Ceiling(TotalAmountOfItems / (double)TotalAmountOfItemsPerPage);
        PageIndex = pageIndex;
        TotalAmountOfItemsPerPage = totalAmountOfItemsPerPage;
        AddRange(items);
    }

    public static async Task<PaginatedList<TItem>> CreateAsync(
        IQueryable<TItem> items,
        int pageIndex,
        int totalAmountOfItemsPerPage,
        CancellationToken cancellationToken)
    {
        int actualTotalAmountOfItemsPerPage = totalAmountOfItemsPerPage > MaxItemsPerPage ? MaxItemsPerPage : totalAmountOfItemsPerPage;
        List<TItem> paginatedItems = await items
            .Skip((pageIndex - 1) * actualTotalAmountOfItemsPerPage)
            .Take(actualTotalAmountOfItemsPerPage)
            .ToListAsync(cancellationToken);
        return new PaginatedList<TItem>(paginatedItems, pageIndex, actualTotalAmountOfItemsPerPage);
    }
}
