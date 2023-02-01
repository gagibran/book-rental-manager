namespace BookRentalManager.Application.Common;

public class GetAllItemsQuery
{
    public int PageIndex { get; }
    public int TotalAmountOfItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetAllItemsQuery(int pageIndex, int totalItemsPerPage, string search)
    {
        PageIndex = pageIndex;
        TotalAmountOfItemsPerPage = totalItemsPerPage;
        SearchParameter = search;
    }
}
