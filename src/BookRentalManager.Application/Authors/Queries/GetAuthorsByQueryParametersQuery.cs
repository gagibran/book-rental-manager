namespace BookRentalManager.Application.Authors.Queries;

public sealed class GetAuthorsByQueryParametersQuery(int pageIndex, int pageSize, string searchParameter, string sortParameters)
    : GetAllItemsQuery(pageIndex, pageSize, searchParameter, sortParameters), IRequest<PaginatedList<GetAuthorDto>>
{
}
