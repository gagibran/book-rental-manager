namespace BookRentalManager.Application.Dtos;

public sealed class GetBookRentedByCustomerDto(string bookTitle, Edition edition, Isbn isbn, DateTime rentedAt, DateTime dueDate)
{
    public string BookTitle { get; } = bookTitle;
    public int Edition { get; } = edition.EditionNumber;
    public string Isbn { get; } = isbn.IsbnValue;
    public DateTime RentedAt { get; } = rentedAt;
    public DateTime DueDate { get; } = dueDate;

    public GetBookRentedByCustomerDto(Book book) : this(
        book.BookTitle,
        book.Edition,
        book.Isbn,
        book.RentedAt!.Value,
        book.DueDate!.Value)
    {
    }
}
