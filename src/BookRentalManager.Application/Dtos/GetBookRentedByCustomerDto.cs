namespace BookRentalManager.Application.Dtos;

[method: JsonConstructor]
public sealed record GetBookRentedByCustomerDto(
    string BookTitle,
    int Edition,
    string Isbn,
    DateTime RentedAt,
    DateTime DueDate)
{
    public GetBookRentedByCustomerDto(Book book) : this(
        book.BookTitle.Title,
        book.Edition.EditionNumber,
        book.Isbn.ToString(),
        book.RentedAt!.Value,
        book.DueDate!.Value)
    {
    }
}
