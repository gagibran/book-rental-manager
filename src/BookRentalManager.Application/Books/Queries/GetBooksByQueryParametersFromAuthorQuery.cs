namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBooksByQueryParametersFromAuthorQuery : GetAllItemsQuery, IQuery<PaginatedList<GetBookDto>>
{
    public Guid AuthorId { get; }

    public GetBooksByQueryParametersFromAuthorQuery(Guid authorId, int pageIndex, int pageSize, string searchParameter, string sortParameters)
        : base(pageIndex, pageSize, searchParameter, sortParameters)
    {
        AuthorId = authorId;
    }
}
