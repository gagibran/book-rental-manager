namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBookByIdQuery : IQuery<GetBookDto>
{
    public Guid BookAuthorId { get; }
    public Guid Id { get; }

    public GetBookByIdQuery(Guid bookAuthorId, Guid id)
    {
        BookAuthorId = bookAuthorId;
        Id = id;
    }
}
