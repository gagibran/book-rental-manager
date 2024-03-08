namespace BookRentalManager.Application.Customers.Queries;

public sealed record GetCustomersByQueryParametersQuery(int PageIndex, int PageSize, string SearchParameter, string SortParameters)
    : GetAllItemsQuery(PageIndex, PageSize, SearchParameter, SortParameters), IRequest<PaginatedList<GetCustomerDto>>;
