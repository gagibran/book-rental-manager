namespace BookRentalManager.Application.Dtos;

public sealed record GetBookRentedByCustomerDto(
    string BookTitle,
    int Edition,
    string Isbn,
    DateTime RentedAt,
    DateTime DueDate)
{
    public GetBookRentedByCustomerDto(Book book) : this(
        book.BookTitle,
        book.Edition.EditionNumber,
        book.Isbn.ToString(),
        book.RentedAt!.Value,
        book.DueDate!.Value)
    {
    }
}
