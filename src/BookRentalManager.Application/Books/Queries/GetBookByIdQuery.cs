namespace BookRentalManager.Application.Books.Queries;

public sealed class GetBookByIdQuery(Guid id) : IRequest<GetBookDto>
{
    public Guid Id { get; } = id;
}
