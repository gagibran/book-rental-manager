namespace SimpleBookManagement.Domain.Entities;

public sealed class BookAuthor : BaseEntity
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
        _books = new List<Book>();
    }

    internal void AddBook(Book book)
    {
        _books.Add(book);
    }
}
