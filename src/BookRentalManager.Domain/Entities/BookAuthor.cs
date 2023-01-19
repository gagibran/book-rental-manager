namespace BookRentalManager.Domain.Entities;

public sealed class BookAuthor : Entity
{
    private readonly List<Book> _books = default!;

    public FullName FullName { get; } = default!;
    public IReadOnlyList<Book> Books => _books;

    private BookAuthor()
    {
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
            return Result.Fail(
                $"The book titled '{book.BookTitle}' has already been added to this author."
            );
        }
        book.AddBookAuthor(this);
        _books.Add(book);
        return Result.Success();
    }
}
