namespace BookRentalManager.Application.Books.Commands;

public sealed class CreateBookCommand : ICommand<BookCreatedDto>
{
    public Guid BookAuthorId { get; }
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }

    public CreateBookCommand(
        Guid bookAuthorId,
        string bookTitle,
        int edition,
        string isbn)
    {
        BookAuthorId = bookAuthorId;
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
    }
}
