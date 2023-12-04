namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBooksByQueryParametersQuery(int pageIndex, int pageSize, string searchParameter, string sortParameters)
    : GetAllItemsQuery(pageIndex, pageSize, searchParameter, sortParameters), IRequest<PaginatedList<GetBookDto>>
{
}
