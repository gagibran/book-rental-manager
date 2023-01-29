namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBooksByQueryParametersFromAuthorQuery : GetAllItemsQuery, IQuery<IReadOnlyList<GetBookDto>>
{
    public Guid AuthorId { get; }

    public GetBooksByQueryParametersFromAuthorQuery(Guid authorId, int pageIndex, int totalItemsPerPage, string searchParameter)
        : base(pageIndex, totalItemsPerPage, searchParameter)
    {
        AuthorId = authorId;
    }
}
