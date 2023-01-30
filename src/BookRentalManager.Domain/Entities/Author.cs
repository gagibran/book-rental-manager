namespace BookRentalManager.Domain.Entities;

public sealed class Author : Entity
{
    private readonly List<Book> _books;

    public FullName FullName { get; }
    public IReadOnlyList<Book> Books => _books.AsReadOnly();

    private Author()
    {
        _books = new();
        FullName = default!;
    }

    public Author(FullName fullName)
    {
        FullName = fullName;
        _books = new();
    }

    public Result AddBook(Book book)
    {
        Book? existingBook = _books.FirstOrDefault(currentBook => currentBook.Isbn.Equals(book.Isbn));
        if (existingBook is not null)
        {
            return Result.Fail("bookIsbn", $"A book with the ISBN '{book.Isbn.IsbnValue}' has already been added to this author.");
        }
        book.AddAuthor(this);
        _books.Add(book);
        return Result.Success();
    }
}
