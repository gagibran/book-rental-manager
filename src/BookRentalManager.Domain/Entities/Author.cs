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

    public void AddBook(Book bookToAdd)
    {
        bookToAdd.AddAuthor(this);
        _books.Add(bookToAdd);
    }
}
