namespace BookRentalManager.Application.Authors.Queries;

public sealed class GetAuthorsByQueryParametersQuery : GetAllItemsQuery, IQuery<PaginatedList<GetAuthorDto>>
{
    public GetAuthorsByQueryParametersQuery(int pageIndex, int pageSize, string searchParameter)
        : base(pageIndex, pageSize, searchParameter)
    {
    }
}
