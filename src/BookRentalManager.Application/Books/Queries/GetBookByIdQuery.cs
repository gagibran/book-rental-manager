namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBookByIdQuery : IQuery<GetBookDto>
{
    public Guid Id { get; }

    public GetBookByIdQuery(Guid id)
    {
        Id = id;
    }
}
