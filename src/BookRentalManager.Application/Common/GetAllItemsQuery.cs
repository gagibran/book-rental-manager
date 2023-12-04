namespace BookRentalManager.Application.Common;

public class GetAllItemsQuery(int pageIndex, int pageSize, string search, string sort)
{
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public string SearchParameter { get; } = search;
    public string SortParameters { get; } = sort;
}
