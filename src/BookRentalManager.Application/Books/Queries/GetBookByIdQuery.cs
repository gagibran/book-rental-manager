namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBookByIdQuery : IRequest<GetBookDto>
{
    public Guid Id { get; }

    public GetBookByIdQuery(Guid id)
    {
        Id = id;
    }
}
