namespace BookRentalManager.Application.Books.Commands;

public sealed class CreateBookCommand(
    IEnumerable<Guid> authorIds,
    string bookTitle,
    int edition,
    string isbn)
    : IRequest<BookCreatedDto>
{
    public IEnumerable<Guid> AuthorIds { get; } = authorIds;
    public string BookTitle { get; } = bookTitle;
    public int Edition { get; } = edition;
    public string Isbn { get; } = isbn;
}
