namespace BookRentalManager.Domain.Entities;

public sealed class Book : Entity
{
    private readonly List<Author> _authors;

    public string BookTitle { get; private set; }
    public IReadOnlyList<Author> Authors => _authors.AsReadOnly();
    public Edition Edition { get; private set; }
    public Isbn Isbn { get; private set; }
    public DateTime? RentedAt { get; internal set; }
    public DateTime? DueDate { get; internal set; }
    public Customer? Customer { get; private set; }

    private Book()
    {
        _authors = [];
        BookTitle = string.Empty!;
        Edition = default!;
        Isbn = default!;
        Customer = default;
        RentedAt = default;
        DueDate = default;
        Customer = default;
    }

    public Book(string bookTitle, Edition edition, Isbn isbn)
    {
        _authors = [];
        BookTitle = bookTitle.Trim();
        Edition = edition;
        Isbn = isbn;
        RentedAt = null;
        DueDate = null;
    }

    public Result UpdateBookTitleEditionAndIsbn(string bookTitle, int edition, string isbn)
    {
        Result<Edition> editionResult = Edition.Create(edition);
        Result<Isbn> isbnResult = Isbn.Create(isbn);
        Result combinedResult = Result.Combine(editionResult, isbnResult);
        if (!combinedResult.IsSuccess)
        {
            return combinedResult;
        }
        BookTitle = bookTitle;
        Edition = editionResult.Value!;
        Isbn = isbnResult.Value!;
        return Result.Success();
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
