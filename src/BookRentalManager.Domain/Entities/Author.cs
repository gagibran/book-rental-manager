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

    public Result AddBook(Book bookToAdd)
    {
        if (_books.Select(book => book.Isbn).Contains(bookToAdd.Isbn))
        {
            return Result.Fail("book", $"A book with the ISBN '{bookToAdd.Isbn}' has already been added to '{FullName}'.");
        }
        bookToAdd.AddAuthor(this);
        _books.Add(bookToAdd);
        return Result.Success();
    }
}
