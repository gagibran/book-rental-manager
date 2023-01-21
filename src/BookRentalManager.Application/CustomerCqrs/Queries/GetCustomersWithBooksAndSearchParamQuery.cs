namespace BookRentalManager.Application.CustomerCqrs.Queries;

public sealed class GetCustomersWithBooksAndSearchParamQuery : IQuery<IReadOnlyList<GetCustomerDto>>
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetCustomersWithBooksAndSearchParamQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
