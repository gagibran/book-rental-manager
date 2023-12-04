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
    public string Isbn { get; } = isbn.IsbnValue;
    public DateTime? RentedAt { get; internal set; } = rentedAt;
    public DateTime? DueDate { get; internal set; } = dueDate;
    public GetCustomerThatRentedBookDto? RentedBy { get; } = rentedBy;
}
