namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBooksByQueryParametersFromAuthorQuery : GetAllItemsQuery, IQuery<PaginatedList<GetBookDto>>
{
    public Guid AuthorId { get; }

    public GetBooksByQueryParametersFromAuthorQuery(Guid authorId, int pageIndex, int pageSize, string searchParameter, string sortParameter)
        : base(pageIndex, pageSize, searchParameter, sortParameter)
    {
        AuthorId = authorId;
    }
}
