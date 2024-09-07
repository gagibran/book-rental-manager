namespace BookRentalManager.Application.Books.Commands;

public sealed record CreateBookCommand(
    IEnumerable<Guid> AuthorIds,
    string BookTitle,
    int Edition,
    string Isbn)
    : IRequest<BookCreatedDto>;
