namespace BookRentalManager.Application.BookCqrs;

public sealed class GetBooksWithAuthorsAndSearchParamQuery : IQuery<IReadOnlyList<GetCustomerDto>>
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }
    public string SearchParameter { get; }

    public GetBooksWithAuthorsAndSearchParamQuery(int pageIndex, int totalItemsPerPage, string searchParameter)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
        SearchParameter = searchParameter;
    }
}
