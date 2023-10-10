namespace BookRentalManager.Application.Authors.Queries;

public sealed class GetAuthorsByQueryParametersQuery : GetAllItemsQuery, IRequest<PaginatedList<GetAuthorDto>>
{
    public GetAuthorsByQueryParametersQuery(int pageIndex, int pageSize, string searchParameter, string sortParameters)
        : base(pageIndex, pageSize, searchParameter, sortParameters)
    {
    }
}
