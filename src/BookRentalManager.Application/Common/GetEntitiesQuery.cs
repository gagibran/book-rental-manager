namespace BookRentalManager.Application.Common;

public class GetEntitiesQuery
{
    private const int MaxTotalItemsPerPage = 50;

    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }

    public GetEntitiesQuery(int pageIndex, int totalItemsPerPage)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage > MaxTotalItemsPerPage ? MaxTotalItemsPerPage : totalItemsPerPage;
    }
}
