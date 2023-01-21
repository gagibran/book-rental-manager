namespace BookRentalManager.Domain.Entities;

public sealed class BookAuthor : Entity
{
    private readonly List<Book> _books;

    public FullName FullName { get; }
    public IReadOnlyList<Book> Books => _books.AsReadOnly();

    private BookAuthor()
    {
        _books = new();
        FullName = default!;
    }

    public BookAuthor(FullName fullName)
    {
        FullName = fullName;
        _books = new();
    }

    public Result AddBook(Book book)
    {
        if (_books.Contains(book))
        {
            return Result.Fail($"The book titled '{book.BookTitle}' has already been added to this author.");
        }
        book.AddBookAuthor(this);
        _books.Add(book);
        return Result.Success();
    }
}
