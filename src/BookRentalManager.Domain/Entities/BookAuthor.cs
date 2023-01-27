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
        Book? existingBook = _books.FirstOrDefault(currentBook => currentBook.Isbn.Equals(book.Isbn));
        if (existingBook is not null)
        {
            return Result.Fail($"A book with the ISBN '{book.Isbn.IsbnValue}' has already been added to this book author.");
        }
        book.AddBookAuthor(this);
        _books.Add(book);
        return Result.Success();
    }
}
