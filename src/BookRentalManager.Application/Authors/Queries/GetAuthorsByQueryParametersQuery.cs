namespace BookRentalManager.Application.Authors.Queries;

public sealed class GetAuthorsByQueryParametersQuery : GetAllItemsQuery, IQuery<PaginatedList<GetAuthorDto>>
{
    public GetAuthorsByQueryParametersQuery(int pageIndex, int pageSize, string searchParameter, string sortParameters)
        : base(pageIndex, pageSize, searchParameter, sortParameters)
    {
    }
}
