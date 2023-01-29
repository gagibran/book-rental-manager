namespace BookRentalManager.Domain.Entities;

public sealed class Book : Entity
{
    private readonly List<Author> _authors;

    public string BookTitle { get; }
    public IReadOnlyList<Author> Authors => _authors.AsReadOnly();
    public Edition Edition { get; }
    public Isbn Isbn { get; }
    public bool IsAvailable { get; internal set; }
    public Customer? Customer { get; private set; }

    private Book()
    {
        _authors = new();
        BookTitle = string.Empty!;
        Edition = default!;
        Isbn = default!;
        IsAvailable = default;
        Customer = default;
    }

    public Book(
        string bookTitle,
        Edition edition,
        Isbn isbn)
    {
        _authors = new();
        BookTitle = bookTitle.Trim();
        Edition = edition;
        Isbn = isbn;
        IsAvailable = true;
    }

    internal void AddAuthor(Author author)
    {
        _authors.Add(author);
    }

    internal void SetRentedBy(Customer customer)
    {
        Customer = customer;
    }
}
