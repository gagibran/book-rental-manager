namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBooksBySearchParameterQuery : IQuery<IReadOnlyList<GetBookDto>>
{
    public Guid BookAuthorId { get; }
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetBooksBySearchParameterQuery(Guid bookAuthorId, int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        BookAuthorId = bookAuthorId;
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
