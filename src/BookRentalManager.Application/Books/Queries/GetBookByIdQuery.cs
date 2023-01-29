namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBookByIdQuery : IQuery<GetBookDto>
{
    public Guid AuthorId { get; }
    public Guid Id { get; }

    public GetBookByIdQuery(Guid authorId, Guid id)
    {
        AuthorId = authorId;
        Id = id;
    }
}
