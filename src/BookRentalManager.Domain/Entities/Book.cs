namespace BookRentalManager.Domain.Entities;

public sealed class Book : Entity
{
    private readonly List<BookAuthor> _bookAuthors;

    public string BookTitle { get; }
    public IReadOnlyList<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();
    public Edition Edition { get; }
    public Isbn Isbn { get; }
    public bool IsAvailable { get; internal set; }
    public Customer? Customer { get; }

    private Book()
    {
        _bookAuthors = default!;
        BookTitle = default!;
        Edition = default!;
        Isbn = default!;
        IsAvailable = default!;
        Customer = default!;
    }

    public Book(
        string bookTitle,
        Edition edition,
        Isbn isbn
    )
    {
        _bookAuthors = new();
        BookTitle = bookTitle;
        Edition = edition;
        Isbn = isbn;
        IsAvailable = true;
    }

    internal void AddBookAuthor(BookAuthor bookAuthor)
    {
        _bookAuthors.Add(bookAuthor);
    }
}
