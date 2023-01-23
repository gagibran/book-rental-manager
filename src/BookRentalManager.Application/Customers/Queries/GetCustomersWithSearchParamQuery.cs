namespace BookRentalManager.Application.Customers.Queries;

public sealed class GetCustomersWithSearchParamQuery : IQuery<IReadOnlyList<GetCustomerDto>>
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetCustomersWithSearchParamQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
