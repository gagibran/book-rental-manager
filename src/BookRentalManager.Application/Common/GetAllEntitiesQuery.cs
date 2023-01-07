namespace BookRentalManager.Application.Common;

public class GetAllEntitiesQuery
{
    private const int MaxTotalItemsPerPage = 50;

    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }

    public GetAllEntitiesQuery(int pageIndex, int totalItemsPerPage)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage > MaxTotalItemsPerPage ? MaxTotalItemsPerPage : totalItemsPerPage;
    }
}
