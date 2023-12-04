namespace BookRentalManager.Application.Customers.Queries;

public sealed class GetCustomersByQueryParametersQuery(int pageIndex, int pageSize, string searchParameter, string sortParameters)
    : GetAllItemsQuery(pageIndex, pageSize, searchParameter, sortParameters), IRequest<PaginatedList<GetCustomerDto>>
{
}
