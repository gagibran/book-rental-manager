namespace BookRentalManager.Application.Common;

public class GetAllItemsQuery
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetAllItemsQuery(int pageIndex, int totalItemsPerPage, string search)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = search;
    }
}
