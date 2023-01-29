namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBookByIdFromAuthorQuery : IQuery<GetBookDto>
{
    public Guid AuthorId { get; }
    public Guid Id { get; }

    public GetBookByIdFromAuthorQuery(Guid authorId, Guid id)
    {
        AuthorId = authorId;
        Id = id;
    }
}
