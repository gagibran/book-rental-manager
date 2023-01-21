namespace BookRentalManager.Application.BookAuthorCqrs.Queries;

public sealed class GetBookAuthorsQuery : IQuery<IReadOnlyList<GetBookAuthorDto>>
{
    public int PageIndex { get; }
    public int TotalItemsPerPage { get; }

    public GetBookAuthorsQuery(int pageIndex, int totalItemsPerPage)
    {
        PageIndex = pageIndex;
        TotalItemsPerPage = totalItemsPerPage;
    }
}
