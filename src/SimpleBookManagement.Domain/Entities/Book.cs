namespace SimpleBookManagement.Domain.Entities;

public sealed class Book : BaseEntity
{
    private readonly List<FullName> _bookAuthors;

    public string BookTitle { get; }
    public IReadOnlyList<FullName> BookAuthors => _bookAuthors;
    public Volume Volume { get; }
    public Isbn Isbn { get; }
    public bool IsAvailable { get; internal set; }

    public Book(
        string bookTitle,
        List<FullName> bookAuthors,
        Volume volume,
        Isbn isbn
    )
    {
        _bookAuthors = bookAuthors;
        BookTitle = bookTitle;
        Volume = volume;
        Isbn = isbn;
        IsAvailable = true;
    }

    public Result AddBookAuthor(FullName bookAuthor)
    {
        if (_bookAuthors.Contains(bookAuthor))
        {
            return Result
                .Fail($"{bookAuthor.CompleteName} has already been added as an author to this book.");
        }
        _bookAuthors.Add(bookAuthor);
        return Result.Success();
    }
}
