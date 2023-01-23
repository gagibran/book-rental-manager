namespace BookRentalManager.Application.BooksAuthors.Queries;

public sealed class GetBookAuthorsWithSearchParamQuery : IQuery<IReadOnlyList<GetBookAuthorDto>>
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetBookAuthorsWithSearchParamQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
