namespace BookRentalManager.Application.Books.Commands;

public sealed class CreateBookCommand : ICommand<BookCreatedDto>
{
    public IEnumerable<Guid> AuthorIds { get; }
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public CreateBookCommand(
        IEnumerable<Guid> authorIds,
        string bookTitle,
        int edition,
        string isbn)
    {
        AuthorIds = authorIds;
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
    }
}
