namespace BookRentalManager.Application.CustomerCqrs.Queries;

public sealed class GetCustomersQuery : IQuery<IReadOnlyList<GetCustomerDto>>
{
    private const int MaxTotalItemsPerPage = 50;

    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }

    public GetCustomersQuery(int pageIndex, int totalItemsPerPage)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage > MaxTotalItemsPerPage ? MaxTotalItemsPerPage : totalItemsPerPage;
    }
}
