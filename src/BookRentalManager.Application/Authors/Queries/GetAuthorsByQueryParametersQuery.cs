namespace BookRentalManager.Application.Authors.Queries;

public sealed class GetAuthorsByQueryParametersQuery : GetAllItemsQuery, IQuery<IReadOnlyList<GetAuthorDto>>
{
    public GetAuthorsByQueryParametersQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
        : base(pageIndex, totalItemsPerPage, searchParameter)
    {
    }
}
