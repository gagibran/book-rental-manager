namespace BookRentalManager.Domain.Entities;

public sealed class Book : Entity
{
    private readonly List<BookAuthor> _bookAuthors = default!;

    public string BookTitle { get; } = default!;
    public IReadOnlyList<BookAuthor> BookAuthors => _bookAuthors;
    public Edition Edition { get; } = default!;
    public Isbn Isbn { get; } = default!;
    public bool IsAvailable { get; internal set; } = default!;
    public Customer? Customer { get; } = default!;

    private Book()
    {
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

    public Result AddBookAuthor(BookAuthor bookAuthor)
    {
        if (_bookAuthors.Contains(bookAuthor))
        {
            return Result
                .Fail($"{bookAuthor.FullName.CompleteName} has already been added as an author to this book.");
        }
        _bookAuthors.Add(bookAuthor);
        bookAuthor.AddBook(this);
        return Result.Success();
    }
}
