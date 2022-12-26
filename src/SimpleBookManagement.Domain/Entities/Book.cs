namespace SimpleBookManagement.Domain.Entities;

public sealed class Book : BaseEntity
{
    public string BookTitle { get; }
    public FullName BookAuthor { get; }
    public Volume Volume { get; }
    public Isbn Isbn { get; }

    public Book(
        string bookTitle,
        FullName bookAuthor,
        Volume volume,
        Isbn isbn
    )
    {
        BookTitle = bookTitle;
        BookAuthor = bookAuthor;
        Volume = volume;
        Isbn = isbn;
    }
}
