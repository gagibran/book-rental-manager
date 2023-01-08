namespace BookRentalManager.Application.BookAuthorCqrs.Queries;

public sealed class GetBookAuthorsQuery : GetEntitiesQuery, IQuery<IReadOnlyList<GetBookAuthorDto>>
{
    public GetBookAuthorsQuery(int pageIndex, int totalItemsPerPage)
        : base(pageIndex, totalItemsPerPage)
    {
    }
}
