namespace BookRentalManager.Application.Books.Commands;

public sealed class CreateBookCommand : ICommand<BookCreatedDto>
{
    public Guid AuthorId { get; }
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public CreateBookCommand(
        Guid authorId,
        string bookTitle,
        int edition,
        string isbn)
    {
        AuthorId = authorId;
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
    }
}
