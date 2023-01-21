namespace BookRentalManager.Application.BookAuthorCqrs.Queries;

public sealed class GetBookAuthorsWithBooksAndSearchParamQuery : IQuery<IReadOnlyList<GetBookAuthorDto>>
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetBookAuthorsWithBooksAndSearchParamQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
