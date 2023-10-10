namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBooksByQueryParametersExcludingFromAuthorQuery : GetAllItemsQuery, IRequest<PaginatedList<GetBookDto>>
{
    public Guid AuthorId { get; }

    public GetBooksByQueryParametersExcludingFromAuthorQuery(Guid authorId, int pageIndex, int pageSize, string searchParameter, string sortParameters)
        : base(pageIndex, pageSize, searchParameter, sortParameters)
    {
        AuthorId = authorId;
    }
}
