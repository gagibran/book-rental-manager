namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBooksBySearchParameterQuery : IQuery<IReadOnlyList<GetBookDto>>
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetBooksBySearchParameterQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
