namespace BookRentalManager.Application.Customers.Queries;

public sealed class GetCustomersByQueryParametersQuery : GetAllItemsQuery, IQuery<PaginatedList<GetCustomerDto>>
{
    public GetCustomersByQueryParametersQuery(int pageIndex, int pageSize, string searchParameter, string sortParameters)
        : base(pageIndex, pageSize, searchParameter, sortParameters)
    {
    }
}
