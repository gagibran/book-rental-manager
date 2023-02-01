namespace BookRentalManager.Application.Authors.Queries;

public sealed class GetAuthorsByQueryParametersQuery : GetAllItemsQuery, IQuery<PaginatedList<GetAuthorDto>>
{
    public GetAuthorsByQueryParametersQuery(int pageIndex, int totalAmountOfItemsPerPage, string searchParameter)
        : base(pageIndex, totalAmountOfItemsPerPage, searchParameter)
    {
    }
}
