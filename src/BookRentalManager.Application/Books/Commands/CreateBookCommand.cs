namespace BookRentalManager.Application.Books.Commands;

public sealed class CreateBookCommand : ICommand
{
    public Guid BookAuthorId { get; }
    public Book Book { get; }

    public CreateBookCommand(Guid bookAuthorId, Book book)
    {
        BookAuthorId = bookAuthorId;
        Book = book;
    }
}
