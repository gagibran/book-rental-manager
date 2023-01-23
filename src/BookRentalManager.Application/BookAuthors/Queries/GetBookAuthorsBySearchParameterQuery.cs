namespace BookRentalManager.Application.BooksAuthors.Queries;

public sealed class GetBookAuthorsBySearchParameterQuery : IQuery<IReadOnlyList<GetBookAuthorDto>>
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetBookAuthorsBySearchParameterQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
