namespace BookRentalManager.Domain.Entities;

public sealed class Author : Entity
{
    private readonly List<Book> _books;

    public FullName FullName { get; }
    public IReadOnlyList<Book> Books => _books.AsReadOnly();

    private Author()
    {
        _books = [];
        FullName = default!;
    }

    public Author(FullName fullName)
    {
        FullName = fullName;
        _books = [];
    }

    public Result AddBook(Book book)
    {
        if (_books.Select(book => book.Id).Contains(book.Id))
        {
            return Result.Fail("book", $"A book with the ISBN '{book.Isbn}' has already been added to this author.");
        }
        book.AddAuthor(this);
        _books.Add(book);
        return Result.Success();
    }
}
