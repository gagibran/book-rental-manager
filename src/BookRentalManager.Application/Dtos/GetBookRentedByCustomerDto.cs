namespace BookRentalManager.Application.Dtos;

public sealed class GetBookRentedByCustomerDto
{
    public string BookTitle { get; }
    public int Edition { get; }
    public string Isbn { get; }
    public DateTime RentedAt { get; }
    public DateTime DueDate { get; }

    public GetBookRentedByCustomerDto(string bookTitle, Edition edition, Isbn isbn, DateTime rentedAt, DateTime dueDate)
    {
        BookTitle = bookTitle;
        Edition = edition.EditionNumber;
        Isbn = isbn.IsbnValue;
        RentedAt = rentedAt;
        DueDate = dueDate;
    }
}
