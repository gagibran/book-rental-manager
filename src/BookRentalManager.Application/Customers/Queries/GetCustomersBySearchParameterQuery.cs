namespace BookRentalManager.Application.Customers.Queries;

public sealed class GetCustomersBySearchParameterQuery : IQuery<IReadOnlyList<GetCustomerDto>>
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetCustomersBySearchParameterQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
