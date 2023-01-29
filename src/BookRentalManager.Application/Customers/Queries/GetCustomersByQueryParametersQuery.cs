namespace BookRentalManager.Application.Customers.Queries;

public sealed class GetCustomersByQueryParametersQuery : GetAllItemsQuery, IQuery<IReadOnlyList<GetCustomerDto>>
{
    public GetCustomersByQueryParametersQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
        : base(pageIndex, totalItemsPerPage, searchParameter)
    {
    }
}
