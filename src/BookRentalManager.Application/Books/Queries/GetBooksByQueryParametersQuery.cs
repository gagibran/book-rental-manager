namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBooksByQueryParametersQuery : GetAllItemsQuery, IQuery<PaginatedList<GetBookDto>>
{
    public GetBooksByQueryParametersQuery(int pageIndex, int pageSize, string searchParameter, string sortParameters)
        : base(pageIndex, pageSize, searchParameter, sortParameters)
    {
    }
}
