namespace BookRentalManager.Application.Dtos;

[method: JsonConstructor]
public sealed record GetBookDto(
    Guid Id,
    string BookTitle,
    IReadOnlyList<GetAuthorFromBookDto> Authors,
    int Edition,
    string Isbn,
    DateTime? RentedAt,
    DateTime? DueDate,
    GetCustomerThatRentedBookDto? RentedBy)
    : IdentifiableDto(Id)
{
    public GetBookDto(Book book) : this(
        book.Id,
        book.BookTitle,
        book.Authors.Select(author => new GetAuthorFromBookDto(author)).ToList().AsReadOnly(),
        book.Edition.EditionNumber,
        book.Isbn.ToString(),
        book.RentedAt,
        book.DueDate,
        book.Customer is not null
            ? new GetCustomerThatRentedBookDto(book.Customer.FullName.ToString(), book.Customer.Email.ToString())
            : null)
    {
    }
}
