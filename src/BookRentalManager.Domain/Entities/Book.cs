namespace BookRentalManager.Domain.Entities;

public sealed class Book : Entity
{
    private readonly List<Author> _authors;

    public string BookTitle { get; }
    public IReadOnlyList<Author> Authors => _authors.AsReadOnly();
    public Edition Edition { get; }
    public Isbn Isbn { get; }
    public DateTime? RentedAt { get; internal set; }
    public DateTime? DueDate { get; internal set; }
    public Customer? Customer { get; private set; }

    private Book()
    {
        _authors = new();
        BookTitle = string.Empty!;
        Edition = default!;
        Isbn = default!;
        Customer = default;
        RentedAt = default;
        DueDate = default;
    }

    public Book(string bookTitle, Edition edition, Isbn isbn)
    {
        _authors = new();
        BookTitle = bookTitle.Trim();
        Edition = edition;
        Isbn = isbn;
        RentedAt = null;
        DueDate = null;
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
