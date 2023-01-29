namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBooksByQueryParametersFromAuthorQuery : IQuery<IReadOnlyList<GetBookDto>>
{
    public Guid AuthorId { get; }
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetBooksByQueryParametersFromAuthorQuery(Guid authorId, int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        AuthorId = authorId;
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
