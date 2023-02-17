namespace BookRentalManager.Application.Common;

public class GetAllItemsQuery
{
    public int PageIndex { get; }
    public int PageSize { get; }
    public string SearchParameter { get; }
    public string SortParameters { get; }

    public GetAllItemsQuery(int pageIndex, int pageSize, string search, string sort)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        SearchParameter = search;
        SortParameters = sort;
    }
}
