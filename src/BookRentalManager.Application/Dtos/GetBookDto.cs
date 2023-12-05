namespace BookRentalManager.Application.Dtos;

public sealed class GetBookDto(
    Guid id,
    string bookTitle,
    IReadOnlyList<GetAuthorFromBookDto> authors,
    Edition edition,
    Isbn isbn,
    DateTime? rentedAt,
    DateTime? dueDate,
    GetCustomerThatRentedBookDto? rentedBy)
    : IdentifiableDto(id)
{
    public string BookTitle { get; } = bookTitle;
    public IReadOnlyList<GetAuthorFromBookDto> Authors { get; } = authors;
    public int Edition { get; } = edition.EditionNumber;
    public string Isbn { get; } = isbn.ToString();
    public DateTime? RentedAt { get; internal set; } = rentedAt;
    public DateTime? DueDate { get; internal set; } = dueDate;
    public GetCustomerThatRentedBookDto? RentedBy { get; } = rentedBy;

    public GetBookDto(Book book) : this(
        book.Id,
        book.BookTitle,
        book.Authors.Select(author => new GetAuthorFromBookDto(author)).ToList().AsReadOnly(),
        book.Edition,
        book.Isbn,
        book.RentedAt,
        book.DueDate,
        book.Customer is not null ? new GetCustomerThatRentedBookDto(book.Customer) : null)
    {
    }
}
